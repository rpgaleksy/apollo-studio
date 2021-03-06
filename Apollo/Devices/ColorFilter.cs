using System;

using Apollo.DeviceViewers;
using Apollo.Elements;
using Apollo.Structures;

namespace Apollo.Devices {
    public class ColorFilter: Device {
        double _h, _s, _v, _th, _ts, _tv;

        public double Hue {
            get => _h;
            set {
                if (-180 <= value && value <= 180 && _h != value) {
                    _h = value;

                    if (Viewer?.SpecificViewer != null) ((ColorFilterViewer)Viewer.SpecificViewer).SetHue(Hue);
                }
            }
        }

        public double Saturation {
            get => _s;
            set {
                if (0 <= value && value <= 1 && _s != value) {
                    _s = value;

                    if (Viewer?.SpecificViewer != null) ((ColorFilterViewer)Viewer.SpecificViewer).SetSaturation(Saturation);
                }
            }
        }

        public double Value {
            get => _v;
            set {
                if (0 <= value && value <= 1 && _v != value) {
                    _v = value;

                    if (Viewer?.SpecificViewer != null) ((ColorFilterViewer)Viewer.SpecificViewer).SetValue(Value);
                }
            }
        }

        public double HueTolerance {
            get => _th;
            set {
                if (0 <= value && value <= 1 && _th != value) {
                    _th = value;

                    if (Viewer?.SpecificViewer != null) ((ColorFilterViewer)Viewer.SpecificViewer).SetHueTolerance(Hue);
                }
            }
        }

        public double SaturationTolerance {
            get => _ts;
            set {
                if (0 <= value && value <= 1 && _ts != value) {
                    _ts = value;

                    if (Viewer?.SpecificViewer != null) ((ColorFilterViewer)Viewer.SpecificViewer).SetSaturationTolerance(Saturation);
                }
            }
        }

        public double ValueTolerance {
            get => _tv;
            set {
                if (0 <= value && value <= 1 && _tv != value) {
                    _tv = value;

                    if (Viewer?.SpecificViewer != null) ((ColorFilterViewer)Viewer.SpecificViewer).SetValueTolerance(Value);
                }
            }
        }

        public override Device Clone() => new ColorFilter(Hue, Saturation, Value, HueTolerance, SaturationTolerance, ValueTolerance) {
            Collapsed = Collapsed,
            Enabled = Enabled
        };

        public ColorFilter(double hue = 0, double saturation = 1, double value = 1, double hue_t = 0.05, double saturation_t = 0.05, double value_t = 0.05): base("colorfilter", "Color Filter") {
            Hue = hue;
            Saturation = saturation;
            Value = value;

            HueTolerance = hue_t;
            SaturationTolerance = saturation_t;
            ValueTolerance = value_t;
        }

        public override void MIDIProcess(Signal n) {
            if (n.Color.Lit) {
                (double hue, double saturation, double value) = n.Color.ToHSV();

                if (!((180 - Math.Abs(Math.Abs(hue - (Hue + 360) % 360) - 180)) / 180 <= HueTolerance &&
                    Math.Abs(saturation - Saturation) <= SaturationTolerance &&
                    Math.Abs(value - Value) <= ValueTolerance)) return;
            }

            InvokeExit(n);
        }
    }
}