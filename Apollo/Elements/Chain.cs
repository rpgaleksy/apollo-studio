using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.IO;
using System.Reflection;
using System.Text;

using Newtonsoft.Json;

using Apollo.Components;
using Apollo.Core;

namespace Apollo.Elements {
    public class Chain: IDeviceParent {
        public static readonly string Identifier = "chain";

        public IChainParent Parent = null;
        public int? ParentIndex;
        private Action<Signal> _midiexit = null;
        public Action<Signal> MIDIExit {
            get {
                return _midiexit;
            }
            set {
                _midiexit = value;
                Reroute();
            }
        }

        private List<Device> _devices = new List<Device>();
        private Action<Signal> _chainenter = null;

        private void Reroute() {
            for (int i = 0; i < _devices.Count; i++) {
                _devices[i].Parent = this;
                _devices[i].ParentIndex = i;
            }
            
            if (_devices.Count == 0)
                _chainenter = _midiexit;

            else {
                _chainenter = _devices[0].MIDIEnter;
                
                for (int i = 1; i < _devices.Count; i++) {
                    _devices[i - 1].MIDIExit = _devices[i].MIDIEnter;
                }
                
                _devices[_devices.Count - 1].MIDIExit = _midiexit;
            }
        }

        public Device this[int index] {
            get {
                return _devices[index];
            }
            set {
                _devices[index] = value;
                Reroute();
            }
        }

        public int Count {
            get {
                return _devices.Count;
            }
        }

        public Chain Clone() {
            return new Chain((from i in _devices select i.Clone()).ToList());
        }

        public void Insert(int index, Device device) {
            _devices.Insert(index, device);
            Reroute();
        }

        public void Add(Device device) {
            _devices.Add(device);
            Reroute();
        }

        public void Add(Device[] devices) {
            foreach (Device device in devices)
                _devices.Add(device);
            Reroute();
        }

        public void Remove(int index) {
            _devices.RemoveAt(index);
            Reroute();
        }

        public Chain(List<Device> init = null) {
            if (init == null) init = new List<Device>();
            _devices = init;
            Reroute();
        }

        public void MIDIEnter(Signal n) {
            _chainenter?.Invoke(n);
        }

        public static Chain Decode(string jsonString) {
            Dictionary<string, object> json = JsonConvert.DeserializeObject<Dictionary<string, object>>(jsonString);
            if (json["object"].ToString() != Identifier) return null;

            List<object> data = JsonConvert.DeserializeObject<List<object>>(json["data"].ToString());
            
            List<Device> init = new List<Device>();
            foreach (object device in data) {
                init.Add(Device.Decode(device.ToString()));
            }
            return new Chain(init);
        }

        public string Encode() {
            StringBuilder json = new StringBuilder();

            using (JsonWriter writer = new JsonTextWriter(new StringWriter(json))) {
                writer.WriteStartObject();

                    writer.WritePropertyName("object");
                    writer.WriteValue(Identifier);

                    writer.WritePropertyName("data");
                    writer.WriteStartArray();

                        for (int i = 0; i < _devices.Count; i++) {
                            writer.WriteRawValue(_devices[i].Encode());
                        }

                    writer.WriteEndArray();

                writer.WriteEndObject();
            }
            
            return json.ToString();
        }
    }
}