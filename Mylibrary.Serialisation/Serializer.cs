using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Formatters.Binary;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Mylibrary.Serialisation
{
    public class Serializer<T>
    {
        public enum Mode
        {
            JSON,
            XML,
            BIN
        }

        private Mode _mode;
        private string _path;
        private Dictionary<Mode, Action<T>> Serializers;
        private Dictionary<Mode, Func<T>> Deserializers;

        public Serializer(Mode mode, string path)
        {
            _mode = mode;
            _path = path;
            Serializers = new Dictionary<Mode, Action<T>>();
            Serializers.Add(Mode.BIN, SerializeBinary);
            Serializers.Add(Mode.XML, SerializeXml);
            Serializers.Add(Mode.JSON, SerializeJson);

            Deserializers = new Dictionary<Mode, Func<T>>();
            Deserializers.Add(Mode.BIN, DeserializeBinary);
            Deserializers.Add(Mode.XML, DeserializeXml);
            Deserializers.Add(Mode.JSON, DeserializeJson);
        }

        #region Serialize
        public void Serialize(T data)
        {
            Serializers[_mode](data);
        }

        private void SerializeJson(T data)
        {
            using (StreamWriter file = new StreamWriter(_path, false))
            {
                JsonSerializer jsonSerializer = new JsonSerializer();
                jsonSerializer.Serialize(file, data);
            }
        }

        private void SerializeXml(T data)
        {
            using (StreamWriter file = new StreamWriter(_path, false))
            {
                var xmlSerializer = new XmlSerializer(typeof(T));
                xmlSerializer.Serialize(file.BaseStream, data);
            }
        }

        private void SerializeBinary(T data)
        {
            using (StreamWriter file = new StreamWriter(_path, false))
            {
                var binaryFormatter = new BinaryFormatter();
                binaryFormatter.Serialize(file.BaseStream, data);
            }
        }
        #endregion

        #region Deserialize
        public T Deserialize()
        {
            return Deserializers[_mode]();
        }

        private T DeserializeBinary()
        {
            using (StreamReader file = new StreamReader(_path))
            {
                var binaryFormatter = new BinaryFormatter();
                return (T)binaryFormatter.Deserialize(file.BaseStream);
            }
        }

        private T DeserializeXml()
        {
            using (StreamReader file = new StreamReader(_path))
            {
                var xmlSerialize = new XmlSerializer(typeof(T));
                return (T)xmlSerialize.Deserialize(file.BaseStream);
            }
        }

        private T DeserializeJson()
        {
            using (StreamReader file = new StreamReader(_path))
            {
                return JsonConvert.DeserializeObject<T>(file.ReadToEnd());
            }
        }
        #endregion
    }
}
