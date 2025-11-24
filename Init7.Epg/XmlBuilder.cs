using System;
using System.Collections.Generic;
using System.ComponentModel.Design.Serialization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using System.Xml;
using System.Diagnostics.CodeAnalysis;
using Init7.Epg.Schema;

#if TARGET_AOT
using Microsoft.Xml.Serialization.GeneratedAssembly;
#endif

namespace Init7.Epg
{
    public abstract class XmlBuilder<T>(T root) where T : notnull
    {
        protected readonly T _root = root;

//#if TARGET_AOT
//        private static readonly XmlSerializerContract _serializers = new();
//#endif

        protected abstract void FinishAppending();

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        private static XmlSerializer GetSerializer(dynamic obj)
        {
            /**
             * in order to build AOT code dealing with XML, we must AOT-generate the XML (de)serializers.
             * to do this, Microsoft.XmlSerializer.Generator needs a .dll that it can load and reflect on.
             * this means we need to build 2 versions of the code, in order to break the cycle:
             * - the AnyCPU (host) version will use the regular JIT-based serializer.
             * - the AOT version will use the generated serializers.
             **/
//#if TARGET_AOT
           // return _serializers.GetSerializer(obj.GetType());
//#else
            return new XmlSerializer(obj.GetType());
//#endif
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public string BuildToString()
        {
            FinishAppending();

            var serializer = new XmlSerializer(typeof(T));// GetSerializer(_root);
            using var stringWriter = new StringWriter();
            using var xmlWriter = XmlWriter.Create(stringWriter);
            serializer.SerializeChecked(xmlWriter, _root);
            return stringWriter.ToString();
        }

        [UnconditionalSuppressMessage("Trimming", "IL2026:Members annotated with 'RequiresUnreferencedCodeAttribute' require dynamic access otherwise can break functionality when trimming application code", Justification = "<Pending>")]
        public void BuildToStream(Stream stream)
        {
            FinishAppending();

            var serializer = GetSerializer(_root);

            using var xmlWriter = XmlWriter.Create(stream, new XmlWriterSettings
            {
                Indent = true
            });

            serializer.Serialize(xmlWriter, _root);

            //serializer.SerializeChecked(xmlWriter, _root);
        }

        public void BuildToFile(string filePath)
        {
            FinishAppending();

            using var stream = new FileStream(
                filePath, FileMode.OpenOrCreate, FileAccess.Write, FileShare.ReadWrite);
            stream.SetLength(0);

            BuildToStream(stream);
        }

        public T Build()
        {
            FinishAppending();
            return _root;
        }
    }
}
