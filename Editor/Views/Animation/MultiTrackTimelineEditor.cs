using System;
using System.Collections.Generic;
using System.Numerics;
using ImGuiNET;

namespace Editor.Views
{
    public static class MultiTrackTimelineEditor
    {
        public class Keyframe
        {
            public float Time;
            public bool Selected;
        }

        public class Track
        {
            public string Name = "";
            public List<Keyframe> Keys = new List<Keyframe>();
        }

        public class EventKeyframe
        {
            public float Time;
            public bool Selected;
            public string Name = "";
        }

        private static List<EventKeyframe> _eventKeys = new List<EventKeyframe>();

        private static float _zoom = 120.0f;
        private static float _scrollX = 0.0f;

        private static float _playheadTime = 0.0f;
        private static bool _draggingPlayhead = false;

        private static bool _draggingKeyframes = false;
        private static bool _draggingEventKeys = false;
        private static bool _boxSelecting = false;

        private static Vector2 _boxSelectStart;
        private static Vector2 _boxSelectEnd;

        private static Vector2 _dragStartMouse;

        private static Dictionary<Keyframe, float> _dragOriginalTimes = new Dictionary<Keyframe, float>();
        private static Dictionary<EventKeyframe, float> _dragOriginalEventTimes = new Dictionary<EventKeyframe, float>();

        private static float _snapStep = 0.1f;
        private static bool _snapEnabled = true;

        private static float _trackHeight = 40.0f;
        private static float _headerHeight = 24.0f;
        private static float _trackLabelWidth = 140.0f;
        private static float _eventTrackHeight = 28.0f;

        public static void Draw(ref List<Track> tracks)
        {
            DrawToolbar();

            Vector2 canvasPos = ImGui.GetCursorScreenPos();
            Vector2 canvasSize = ImGui.GetContentRegionAvail();

            if (canvasSize.X < 50.0f)
            {
                canvasSize.X = 50.0f;
            }

            if (canvasSize.Y < 50.0f)
            {
                canvasSize.Y = 50.0f;
            }

            ImDrawListPtr drawList = ImGui.GetWindowDrawList();

            ImGui.InvisibleButton("timeline_canvas", canvasSize, ImGuiButtonFlags.MouseButtonLeft | ImGuiButtonFlags.MouseButtonMiddle);

            bool hovered = ImGui.IsItemHovered();
            bool active = ImGui.IsItemActive();

            Vector2 mousePos = ImGui.GetIO().MousePos;

            drawList.AddRectFilled(canvasPos, canvasPos + canvasSize, ImGui.GetColorU32(new Vector4(0.10f, 0.10f, 0.10f, 1.0f)));
            drawList.AddRect(canvasPos, canvasPos + canvasSize, ImGui.GetColorU32(new Vector4(0.35f, 0.35f, 0.35f, 1.0f)));

            if (hovered)
            {
                HandleZoom(mousePos, canvasPos);
            }

            if (active && ImGui.IsMouseDragging(ImGuiMouseButton.Middle))
            {
                _scrollX += ImGui.GetIO().MouseDelta.X;
            }

            DrawTimeHeader(drawList, canvasPos, canvasSize);
            DrawEventTrack(drawList, canvasPos, canvasSize);
            DrawTracks(drawList, canvasPos, canvasSize, tracks);
            DrawPlayhead(drawList, canvasPos, canvasSize);

            if (hovered || _draggingPlayhead || _draggingKeyframes || _draggingEventKeys || _boxSelecting)
            {
                HandleInput(drawList, mousePos, canvasPos, canvasSize, tracks);
            }
        }

        private static void DrawToolbar()
        {
            ImGui.Text("Timeline Controls");

            ImGui.SameLine();
            ImGui.Checkbox("Snap", ref _snapEnabled);

            ImGui.SameLine();
            ImGui.SetNextItemWidth(80);
            ImGui.DragFloat("Step", ref _snapStep, 0.01f, 0.01f, 10.0f);

            ImGui.SameLine();
            ImGui.SetNextItemWidth(120);
            ImGui.DragFloat("Zoom", ref _zoom, 1.0f, 20.0f, 500.0f);

            ImGui.SameLine();
            ImGui.SetNextItemWidth(120);
            ImGui.DragFloat("Playhead", ref _playheadTime, 0.01f, 0.0f, 9999.0f);

            ImGui.Separator();
        }

        private static float TimeToScreenX(float time, float originX)
        {
            return originX + _trackLabelWidth + _scrollX + time * _zoom;
        }

        private static float ScreenXToTime(float x, float originX)
        {
            return (x - originX - _trackLabelWidth - _scrollX) / _zoom;
        }

        private static float SnapTime(float time)
        {
            if (!_snapEnabled)
            {
                return time;
            }

            if (_snapStep <= 0.0f)
            {
                return time;
            }

            return MathF.Round(time / _snapStep) * _snapStep;
        }

        private static void HandleZoom(Vector2 mousePos, Vector2 canvasPos)
        {
            float wheel = ImGui.GetIO().MouseWheel;

            if (wheel == 0.0f)
            {
                return;
            }

            float originX = canvasPos.X;

            float mouseTimeBefore = ScreenXToTime(mousePos.X, originX);

            _zoom *= (1.0f + wheel * 0.1f);

            if (_zoom < 20.0f)
            {
                _zoom = 20.0f;
            }

            if (_zoom > 500.0f)
            {
                _zoom = 500.0f;
            }

            float mouseTimeAfter = ScreenXToTime(mousePos.X, originX);
            float timeDiff = mouseTimeAfter - mouseTimeBefore;

            _scrollX += timeDiff * _zoom;
        }

        private static void DrawTimeHeader(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
        {
            var headerMin = canvasPos;
            var headerMax = new Vector2(canvasPos.X + canvasSize.X, canvasPos.Y + _headerHeight);

            drawList.AddRectFilled(headerMin, headerMax, ImGui.GetColorU32(new Vector4(0.14f, 0.14f, 0.14f, 1)));

            drawList.AddLine(new Vector2(canvasPos.X + _trackLabelWidth, canvasPos.Y),
                             new Vector2(canvasPos.X + _trackLabelWidth, canvasPos.Y + canvasSize.Y),
                             ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 1)));

            float majorStep = 1.0f;
            float minorStep = 0.1f;

            if (_zoom < 50.0f)
            {
                majorStep = 2.0f;
                minorStep = 0.5f;
            }

            if (_zoom > 200.0f)
            {
                majorStep = 0.5f;
                minorStep = 0.1f;
            }

            float originX = canvasPos.X;

            float timeStart = (-_scrollX) / _zoom;
            if (timeStart < 0.0f)
            {
                timeStart = 0.0f;
            }

            float firstMajor = MathF.Floor(timeStart / majorStep) * majorStep;

            for (float t = firstMajor; ; t += minorStep)
            {
                float x = TimeToScreenX(t, originX);

                if (x > canvasPos.X + canvasSize.X)
                {
                    break;
                }

                bool major = MathF.Abs((t / majorStep) - MathF.Round(t / majorStep)) < 0.001f;

                uint color = major
                    ? ImGui.GetColorU32(new Vector4(0.4f, 0.4f, 0.4f, 1))
                    : ImGui.GetColorU32(new Vector4(0.25f, 0.25f, 0.25f, 1));

                if (x >= canvasPos.X + _trackLabelWidth)
                {
                    drawList.AddLine(new Vector2(x, canvasPos.Y), new Vector2(x, canvasPos.Y + canvasSize.Y), color);
                }

                if (major)
                {
                    drawList.AddText(new Vector2(x + 2, canvasPos.Y + 2), ImGui.GetColorU32(new Vector4(1, 1, 1, 1)), $"{t:0.0}s");
                }
            }

            drawList.AddRectFilled(canvasPos, new Vector2(canvasPos.X + _trackLabelWidth, canvasPos.Y + _headerHeight),
                                   ImGui.GetColorU32(new Vector4(0.16f, 0.16f, 0.16f, 1)));

            drawList.AddLine(new Vector2(canvasPos.X + _trackLabelWidth, canvasPos.Y),
                             new Vector2(canvasPos.X + _trackLabelWidth, canvasPos.Y + _headerHeight),
                             ImGui.GetColorU32(new Vector4(0.3f, 0.3f, 0.3f, 1)));
        }

        private static void DrawEventTrack(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
        {
            float originX = canvasPos.X;

            float trackTop = canvasPos.Y + _headerHeight;
            float trackBottom = trackTop + _eventTrackHeight;

            var trackMin = new Vector2(canvasPos.X, trackTop);
            var trackMax = new Vector2(canvasPos.X + canvasSize.X, trackBottom);

            drawList.AddRectFilled(trackMin, trackMax, ImGui.GetColorU32(new Vector4(0.13f, 0.50f, 0.10f, 1.0f)));

            drawList.AddLine(new Vector2(canvasPos.X, trackBottom), new Vector2(canvasPos.X + canvasSize.X, trackBottom),
                             ImGui.GetColorU32(new Vector4(0.25f, 0.25f, 0.25f, 1)));

            drawList.AddRectFilled(trackMin, new Vector2(canvasPos.X + _trackLabelWidth, trackBottom),
                                   ImGui.GetColorU32(new Vector4(1.0f, 0.12f, 0.12f, 1)));

            drawList.AddText(new Vector2(canvasPos.X + 6, trackTop + 6),
                             ImGui.GetColorU32(new Vector4(1, 1, 1, 1)), "Events");

            var clipMin = new Vector2(canvasPos.X + _trackLabelWidth, trackTop);
            var clipMax = new Vector2(canvasPos.X + canvasSize.X, trackBottom);

            drawList.PushClipRect(clipMin, clipMax, true);

            foreach (EventKeyframe k in _eventKeys)
            {
                float x = TimeToScreenX(k.Time, originX);
                float y = trackTop + _eventTrackHeight * 0.5f;

                float size = 6.0f;

                uint col = k.Selected ? ImGui.GetColorU32(new Vector4(1.0f, 0.6f, 0.2f, 1.0f))
                                      : ImGui.GetColorU32(new Vector4(1.0f, 0.3f, 0.3f, 1.0f));

                var p0 = new Vector2(x, y + size);
                var p1 = new Vector2(x + size, y - size);
                var p2 = new Vector2(x - size, y - size);

                drawList.AddTriangleFilled(p0, p1, p2, col);
                drawList.AddTriangle(p0, p1, p2, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));
            }

            drawList.PopClipRect();
        }

        private static void DrawTracks(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize, List<Track> tracks)
        {
            float startY = canvasPos.Y + _headerHeight + _eventTrackHeight;
            float originX = canvasPos.X;

            for (int t = 0; t < tracks.Count; t++)
            {
                float trackTop = startY + t * _trackHeight;
                float trackBottom = trackTop + _trackHeight;

                if (trackBottom > canvasPos.Y + canvasSize.Y)
                {
                    break;
                }

                var trackMin = new Vector2(canvasPos.X, trackTop);
                var trackMax = new Vector2(canvasPos.X + canvasSize.X, trackBottom);

                var bgColor = (t % 2 == 0) ? new Vector4(0.12f, 0.12f, 0.12f, 1) : new Vector4(0.10f, 0.10f, 0.10f, 1);

                drawList.AddRectFilled(trackMin, trackMax, ImGui.GetColorU32(bgColor));

                drawList.AddLine(new Vector2(canvasPos.X, trackBottom), new Vector2(canvasPos.X + canvasSize.X, trackBottom),
                                 ImGui.GetColorU32(new Vector4(0.25f, 0.25f, 0.25f, 1)));

                var clipMin = new Vector2(canvasPos.X + _trackLabelWidth, trackTop);
                var clipMax = new Vector2(canvasPos.X + canvasSize.X, trackBottom);

                drawList.PushClipRect(clipMin, clipMax, true);

                foreach (Keyframe k in tracks[t].Keys)
                {
                    float x = TimeToScreenX(k.Time, originX);
                    float y = trackTop + _trackHeight * 0.5f;

                    float size = 6.0f;

                    uint col = k.Selected ? ImGui.GetColorU32(new Vector4(1.0f, 0.8f, 0.2f, 1.0f))
                                          : ImGui.GetColorU32(new Vector4(0.2f, 0.8f, 1.0f, 1.0f));

                    var p0 = new Vector2(x, y - size);
                    var p1 = new Vector2(x + size, y);
                    var p2 = new Vector2(x, y + size);
                    var p3 = new Vector2(x - size, y);

                    drawList.AddQuadFilled(p0, p1, p2, p3, col);
                    drawList.AddQuad(p0, p1, p2, p3, ImGui.GetColorU32(new Vector4(0, 0, 0, 1)));
                }

                drawList.PopClipRect();

                drawList.AddRectFilled(trackMin, new Vector2(canvasPos.X + _trackLabelWidth, trackBottom),
                                       ImGui.GetColorU32(new Vector4(0.16f, 0.16f, 0.16f, 1)));

                drawList.AddText(new Vector2(canvasPos.X + 6, trackTop + 10),
                                 ImGui.GetColorU32(new Vector4(1, 1, 1, 1)), tracks[t].Name);
            }
        }

        private static void DrawPlayhead(ImDrawListPtr drawList, Vector2 canvasPos, Vector2 canvasSize)
        {
            float originX = canvasPos.X;
            float x = TimeToScreenX(_playheadTime, originX);

            if (x < canvasPos.X + _trackLabelWidth)
            {
                return;
            }

            if (x > canvasPos.X + canvasSize.X)
            {
                return;
            }

            var playheadColor = new Vector4(1, 1.0f, 1.0f, 1);
            drawList.AddLine(new Vector2(x, canvasPos.Y), new Vector2(x, canvasPos.Y + canvasSize.Y), ImGui.GetColorU32(playheadColor), 2.0f);

            float handleSize = 4.0f;

            var handleMin = new Vector2(x - handleSize, canvasPos.Y);
            var handleMax = new Vector2(x + handleSize, canvasPos.Y + _headerHeight - 2);

            drawList.AddRectFilled(handleMin, handleMax, ImGui.GetColorU32(playheadColor));
        }

        private static void HandleInput(ImDrawListPtr drawList, Vector2 mousePos, Vector2 canvasPos, Vector2 canvasSize, List<Track> tracks)
        {
            float originX = canvasPos.X;
            float timelineLeft = canvasPos.X + _trackLabelWidth;
            float timelineTop = canvasPos.Y + _headerHeight;
            float timelineBottom = canvasPos.Y + canvasSize.Y;

            if (!_draggingPlayhead && !_draggingKeyframes && !_draggingEventKeys && !_boxSelecting)
            {
                if (mousePos.Y < canvasPos.Y || mousePos.Y > timelineBottom)
                {
                    return;
                }
            }

            if (ImGui.IsMouseClicked(ImGuiMouseButton.Left))
            {
                float playheadX = TimeToScreenX(_playheadTime, originX);

                float handleSize = 8.0f;
                var handleMin = new Vector2(playheadX - handleSize, canvasPos.Y + 4);
                var handleMax = new Vector2(playheadX + handleSize, canvasPos.Y + _headerHeight - 2);

                bool clickedPlayheadHandle = mousePos.X >= handleMin.X && mousePos.X <= handleMax.X &&
                                             mousePos.Y >= handleMin.Y && mousePos.Y <= handleMax.Y;

                bool clickedHeader = mousePos.Y >= canvasPos.Y && mousePos.Y <= canvasPos.Y + _headerHeight;

                if (clickedPlayheadHandle)
                {
                    _draggingPlayhead = true;
                }
                else if (clickedHeader && mousePos.X >= timelineLeft)
                {
                    float time = ScreenXToTime(mousePos.X, originX);
                    if (time < 0.0f)
                    {
                        time = 0.0f;
                    }

                    _playheadTime = SnapTime(time);
                    _draggingPlayhead = true;
                }
                else
                {
                    bool ctrl = ImGui.GetIO().KeyCtrl;
                    bool shift = ImGui.GetIO().KeyShift;

                    EventKeyframe clickedEvent = FindEventKeyframeAt(mousePos, canvasPos);

                    if (clickedEvent != null)
                    {
                        if (!ctrl && !shift && !clickedEvent.Selected)
                        {
                            ClearSelection(tracks);
                            ClearEventSelection();
                        }

                        if (ctrl)
                        {
                            clickedEvent.Selected = !clickedEvent.Selected;
                        }
                        else
                        {
                            if (!clickedEvent.Selected)
                            {
                                ClearEventSelection();
                                clickedEvent.Selected = true;
                            }
                        }

                        _draggingEventKeys = true;
                        _dragStartMouse = mousePos;
                        CacheSelectedEventOriginalTimes();
                    }
                    else
                    {
                        int clickedTrackIndex = GetTrackIndex(mousePos.Y, canvasPos.Y, tracks.Count);
                        Keyframe clickedKey = FindKeyframeAt(mousePos, canvasPos, tracks, clickedTrackIndex);

                        if (!ctrl && !shift && clickedKey == null)
                        {
                            ClearSelection(tracks);
                            ClearEventSelection();
                        }

                        if (clickedKey != null)
                        {
                            if (ctrl)
                            {
                                clickedKey.Selected = !clickedKey.Selected;
                            }
                            else
                            {
                                if (!clickedKey.Selected)
                                {
                                    ClearSelection(tracks);
                                    ClearEventSelection();
                                    clickedKey.Selected = true;
                                }
                            }

                            _draggingKeyframes = true;
                            _dragStartMouse = mousePos;
                            CacheSelectedOriginalTimes(tracks);
                        }
                        else
                        {
                            if (mousePos.X >= timelineLeft && mousePos.Y >= timelineTop)
                            {
                                _boxSelecting = true;
                                _boxSelectStart = mousePos;
                                _boxSelectEnd = mousePos;
                            }
                        }
                    }
                }
            }

            if (_draggingPlayhead && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                float time = ScreenXToTime(mousePos.X, originX);
                if (time < 0.0f)
                {
                    time = 0.0f;
                }

                _playheadTime = SnapTime(time);
            }

            if (_draggingKeyframes && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                float deltaPixels = mousePos.X - _dragStartMouse.X;
                float deltaTime = deltaPixels / _zoom;

                foreach (Track tr in tracks)
                {
                    foreach (Keyframe k in tr.Keys)
                    {
                        if (!k.Selected)
                        {
                            continue;
                        }

                        float original = _dragOriginalTimes[k];
                        float newTime = original + deltaTime;

                        if (newTime < 0.0f)
                        {
                            newTime = 0.0f;
                        }

                        k.Time = SnapTime(newTime);
                    }
                }
            }

            if (_draggingEventKeys && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                float deltaPixels = mousePos.X - _dragStartMouse.X;
                float deltaTime = deltaPixels / _zoom;

                foreach (EventKeyframe k in _eventKeys)
                {
                    if (!k.Selected)
                    {
                        continue;
                    }

                    float original = _dragOriginalEventTimes[k];
                    float newTime = original + deltaTime;

                    if (newTime < 0.0f)
                    {
                        newTime = 0.0f;
                    }

                    k.Time = SnapTime(newTime);
                }
            }

            if (_boxSelecting && ImGui.IsMouseDown(ImGuiMouseButton.Left))
            {
                _boxSelectEnd = mousePos;

                var min = new Vector2(MathF.Min(_boxSelectStart.X, _boxSelectEnd.X), MathF.Min(_boxSelectStart.Y, _boxSelectEnd.Y));
                var max = new Vector2(MathF.Max(_boxSelectStart.X, _boxSelectEnd.X), MathF.Max(_boxSelectStart.Y, _boxSelectEnd.Y));

                drawList.AddRectFilled(min, max, ImGui.GetColorU32(new Vector4(0.3f, 0.6f, 1.0f, 0.15f)));
                drawList.AddRect(min, max, ImGui.GetColorU32(new Vector4(0.3f, 0.6f, 1.0f, 0.8f)));

                bool ctrl = ImGui.GetIO().KeyCtrl;

                if (!ctrl)
                {
                    ClearSelection(tracks);
                    ClearEventSelection();
                }

                foreach (EventKeyframe k in _eventKeys)
                {
                    float x = TimeToScreenX(k.Time, originX);
                    float y = canvasPos.Y + _headerHeight + _eventTrackHeight * 0.5f;

                    if (x >= min.X && x <= max.X && y >= min.Y && y <= max.Y)
                    {
                        k.Selected = true;
                    }
                }

                foreach (Track tr in tracks)
                {
                    foreach (Keyframe k in tr.Keys)
                    {
                        float x = TimeToScreenX(k.Time, originX);

                        int trackIndex = tracks.IndexOf(tr);
                        float trackTop = canvasPos.Y + _headerHeight + _eventTrackHeight + trackIndex * _trackHeight;
                        float y = trackTop + _trackHeight * 0.5f;

                        if (x >= min.X && x <= max.X && y >= min.Y && y <= max.Y)
                        {
                            k.Selected = true;
                        }
                    }
                }
            }

            if (ImGui.IsMouseReleased(ImGuiMouseButton.Left))
            {
                _draggingPlayhead = false;
                _draggingKeyframes = false;
                _draggingEventKeys = false;
                _boxSelecting = false;
            }

            if (ImGui.IsKeyPressed(ImGuiKey.Delete))
            {
                DeleteSelectedKeys(tracks);
                DeleteSelectedEventKeys();
            }

            if (ImGui.IsMouseDoubleClicked(ImGuiMouseButton.Left))
            {
                float timelineStartY = canvasPos.Y + _headerHeight;
                float eventBottom = timelineStartY + _eventTrackHeight;

                if (mousePos.Y >= timelineStartY && mousePos.Y <= eventBottom && mousePos.X >= timelineLeft)
                {
                    float time = ScreenXToTime(mousePos.X, originX);

                    if (time < 0.0f)
                    {
                        time = 0.0f;
                    }

                    time = SnapTime(time);

                    _eventKeys.Add(new EventKeyframe { Time = time, Name = "Event" });
                }
                else
                {
                    int clickedTrackIndex = GetTrackIndex(mousePos.Y, canvasPos.Y, tracks.Count);

                    if (clickedTrackIndex >= 0 && clickedTrackIndex < tracks.Count && mousePos.X >= timelineLeft)
                    {
                        float time = ScreenXToTime(mousePos.X, originX);

                        if (time < 0.0f)
                        {
                            time = 0.0f;
                        }

                        time = SnapTime(time);

                        tracks[clickedTrackIndex].Keys.Add(new Keyframe { Time = time });
                    }
                }
            }
        }

        private static int GetTrackIndex(float mouseY, float canvasY, int trackCount)
        {
            float startY = canvasY + _headerHeight + _eventTrackHeight;
            float localY = mouseY - startY;

            if (localY < 0.0f)
            {
                return -1;
            }

            int index = (int)(localY / _trackHeight);

            if (index < 0 || index >= trackCount)
            {
                return -1;
            }

            return index;
        }

        private static Keyframe FindKeyframeAt(Vector2 mousePos, Vector2 canvasPos, List<Track> tracks, int trackIndex)
        {
            if (trackIndex < 0 || trackIndex >= tracks.Count)
            {
                return null;
            }

            float originX = canvasPos.X;
            float trackTop = canvasPos.Y + _headerHeight + _eventTrackHeight + trackIndex * _trackHeight;
            float y = trackTop + _trackHeight * 0.5f;

            float keySize = 6.0f;

            foreach (Keyframe k in tracks[trackIndex].Keys)
            {
                float x = TimeToScreenX(k.Time, originX);

                if (MathF.Abs(mousePos.X - x) <= keySize && MathF.Abs(mousePos.Y - y) <= keySize)
                {
                    return k;
                }
            }

            return null;
        }

        private static EventKeyframe FindEventKeyframeAt(Vector2 mousePos, Vector2 canvasPos)
        {
            float originX = canvasPos.X;
            float trackTop = canvasPos.Y + _headerHeight;
            float y = trackTop + _eventTrackHeight * 0.5f;

            float keySize = 6.0f;

            foreach (EventKeyframe k in _eventKeys)
            {
                float x = TimeToScreenX(k.Time, originX);

                if (MathF.Abs(mousePos.X - x) <= keySize && MathF.Abs(mousePos.Y - y) <= keySize)
                {
                    return k;
                }
            }

            return null;
        }

        private static void ClearSelection(List<Track> tracks)
        {
            foreach (Track tr in tracks)
            {
                foreach (Keyframe k in tr.Keys)
                {
                    k.Selected = false;
                }
            }
        }

        private static void ClearEventSelection()
        {
            foreach (EventKeyframe k in _eventKeys)
            {
                k.Selected = false;
            }
        }

        private static void CacheSelectedOriginalTimes(List<Track> tracks)
        {
            _dragOriginalTimes.Clear();

            foreach (Track tr in tracks)
            {
                foreach (Keyframe k in tr.Keys)
                {
                    if (k.Selected)
                    {
                        _dragOriginalTimes[k] = k.Time;
                    }
                }
            }
        }

        private static void CacheSelectedEventOriginalTimes()
        {
            _dragOriginalEventTimes.Clear();

            foreach (EventKeyframe k in _eventKeys)
            {
                if (k.Selected)
                {
                    _dragOriginalEventTimes[k] = k.Time;
                }
            }
        }

        private static void DeleteSelectedKeys(List<Track> tracks)
        {
            foreach (Track tr in tracks)
            {
                tr.Keys.RemoveAll(k => k.Selected);
            }
        }

        private static void DeleteSelectedEventKeys()
        {
            _eventKeys.RemoveAll(k => k.Selected);
        }
    }
}