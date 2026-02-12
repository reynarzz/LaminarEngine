using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Engine.Utils
{
    internal class FileUtils
    {
        internal static unsafe Guid ReadGuidNoAlloc(BinaryReader reader)
        {
            Guid guid;
            reader.Read(new Span<byte>(&guid, sizeof(Guid)));
            return guid;
        }
    }
}
