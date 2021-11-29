using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EpShootingGame
{
    public class EpKeyState
    {
        public void KeyDownState(KeyEventArgs e, EpPlayer player)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Left)
                player.AddDir(Dir.Left);
            if (e.KeyCode == System.Windows.Forms.Keys.Right)
                player.AddDir(Dir.Right);
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
                player.AddDir(Dir.Up);
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
                player.AddDir(Dir.Down);
        }

        public void KeyUpState(KeyEventArgs e, EpPlayer player)
        {
            if (e.KeyCode == System.Windows.Forms.Keys.Left)
                player.RemoveDir(Dir.Left);
            if (e.KeyCode == System.Windows.Forms.Keys.Right)
                player.RemoveDir(Dir.Right);
            if (e.KeyCode == System.Windows.Forms.Keys.Up)
                player.RemoveDir(Dir.Up);
            if (e.KeyCode == System.Windows.Forms.Keys.Down)
                player.RemoveDir(Dir.Down);
        }

    }
}
