using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Windows.Forms;

internal static class Program
{
    [StructLayout(LayoutKind.Sequential)]
    private struct MouseInput
    {
        public int dx, dy;
        public uint mouseData, flags, time;
        public UIntPtr extra;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct KeyboardInput
    {
        public ushort key, scan;
        public uint flags, time;
        public UIntPtr extra;
    }

    [StructLayout(LayoutKind.Explicit)]
    private struct InputUnion
    {
        [FieldOffset(0)] public MouseInput mouse;
        [FieldOffset(0)] public KeyboardInput keyboard;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Input
    {
        public uint type;
        public InputUnion data;
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern uint SendInput(uint count, Input[] inputs, int size);

    private static Input Key(ushort key, bool up)
    {
        // Windows key is an extended key.
        return new Input {
            type = 1,
            data = new InputUnion {
                keyboard = new KeyboardInput {
                    key = key, flags = (up ? 2u : 0u) | (key == 0x5B ? 1u : 0u)
                }
            }
        };
    }

    [STAThread]
    private static void Main()
    {
        Application.EnableVisualStyles();
        Application.SetCompatibleTextRenderingDefault(false);
        using (var form = new Form())
        {
            form.Text = "描画リセット";
            form.Font = new Font("Yu Gothic UI", 10);
            form.AutoScaleMode = AutoScaleMode.Dpi;
            form.ClientSize = new Size(450, 220);
            form.FormBorderStyle = FormBorderStyle.FixedDialog;
            form.MaximizeBox = false;
            form.StartPosition = FormStartPosition.CenterScreen;

            var description = new Label {
                Location = new Point(20, 18), Size = new Size(410, 70),
                Text = "このPCに Win + Ctrl + Shift + B を送ります。\n画面が一瞬暗くなったり、音が鳴る場合があります。\n操作したいリモートPC側で実行してください。"
            };
            var button = new Button {
                Location = new Point(20, 98), Size = new Size(410, 48),
                Text = "描画をリセット", UseVisualStyleBackColor = true
            };
            var status = new Label {
                Location = new Point(20, 159), Size = new Size(410, 48),
                Text = "GPUデバイスの再起動やWindowsの再起動は行いません。"
            };
            var timer = new Timer { Interval = 500 };
            button.Click += delegate {
                button.Enabled = false;
                status.Text = "キーを送信しています…";
                timer.Start();
            };
            timer.Tick += delegate {
                timer.Stop();
                ushort[] keys = { 0x5B, 0x11, 0x10, 0x42 };
                var inputs = new Input[8];
                for (int i = 0; i < 4; i++) {
                    inputs[i] = Key(keys[i], false);
                    inputs[4 + i] = Key(keys[3 - i], true);
                }
                uint sent = SendInput(8, inputs, Marshal.SizeOf(typeof(Input)));
                if (sent == 8) {
                    status.Text = "送信しました（" + DateTime.Now.ToString("HH:mm:ss") + "）。\nリセット完了を検出する機能はありません。";
                } else {
                    int error = Marshal.GetLastWin32Error();
                    var release = new Input[4];
                    for (int i = 0; i < 4; i++) release[i] = Key(keys[3 - i], true);
                    SendInput(4, release, Marshal.SizeOf(typeof(Input)));
                    status.Text = "キーを送信できませんでした。送信数: " + sent + "/8、エラー: " + error;
                }
                button.Enabled = true;
            };
            form.FormClosed += delegate { timer.Dispose(); };
            form.Controls.AddRange(new Control[] { description, button, status });
            Application.Run(form);
        }
    }
}
