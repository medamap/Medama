#if MEDAMA_USE_UNIRX && MEDAMA_USE_OBSERVABLE_SSH
using System.Linq;
using System.IO;

namespace Medama.ObservableSsh
{
    /// <summary>
    /// ssh stream buffer class
    /// </summary>
    public class Buffer
    {
        /// <summary>
        /// ssh read buffer
        /// </summary>
        public char[] buffer = Enumerable.Repeat((char)0, 65536).ToArray();

        /// <summary>
        /// write index of buffer
        /// </summary>
        public int index = 0;

        /// <summary>
        /// use count of ssh read buffer
        /// </summary>
        public int count = 0;

        /// <summary>
        /// read from stream
        /// </summary>
        public Buffer UpdateFromReadStream(StreamReader reader) {
            var count_up = reader.Read(buffer, index, buffer.Length - index - 1);
            count += count_up;
            index += count_up;
            return this;
        }

        /// <summary>
        /// get line from ssh read buffer
        /// </summary>
        public string ReadLine(bool forceReadLine = false) {

            var newline = (string)null;

            // Force read line
            if (forceReadLine) {
                index = 0;
                count = 0;
                newline = new string(buffer);
                for (int ii = 0; ii < count; ii++) {
                    buffer[ii] = (char)0;
                }
                return newline;
            }

            // search index of CR LF code (-1 is not found)
            var find = Enumerable.Range(0, count).Where(index => buffer[index] == '\n' || buffer[index] == '\r').Select(i => i + 1).FirstOrDefault() - 1;
            // length of line chars
            var length = find;

            //Debug.LogFormat("count = {0} / find = {1}", count, find);

            // find CR LF
            if (find >= 0) {
                var backup_char = buffer[find];        // back up character
                buffer[find] = (char)0;                // set \0
                newline = new string(buffer, 0, find); // create new string of line
                buffer[find] = backup_char;            // restore character

                var src = find;
                var dst = 0;

                //Debug.LogFormat("*** BEFORE *** {0}", string.Join(" / ", Enumerable.Range(0, count).Select(x => string.Format("{0}:{1} \"{2}\"", x, ((int)buffer[x]).ToString(), new string(buffer[x], 1))).ToArray()));

                // Skip first CR LF
                while(src < count && (buffer[src] == '\r' || buffer[src] == '\n')) {
                    src++;
                    length++;
                }

                // GC
                while (src < count) {
                    buffer[dst] = buffer[src];
                    buffer[src] = (char)0;
                    src++;
                    dst++;
                }

                // fill \0
                for (int remaining = count - length; remaining < length + 1; remaining++) {
                    buffer[remaining] = (char)0;
                }

                //Debug.LogFormat("*** find *** \"{0}\" / find = {1} / count = {2} / index = {3} / length = {4}", newline, find, count, index, length);
                //Debug.LogFormat("*** AFTER *** {0}", string.Join(" ", Enumerable.Range(0, count).Select(x => string.Format("{0}:{1} \"{2}\"", x, ((int)buffer[x]).ToString(), new string(buffer[x], 1))).ToArray()));
                count -= length;
                index -= length;
                //Debug.LogFormat("----- count = {0} / index = {1} -----", count, index);

            }
            return newline;
        }
    }
}
#endif
