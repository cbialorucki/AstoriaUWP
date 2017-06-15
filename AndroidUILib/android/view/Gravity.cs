using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AndroidInteropLib.android.view
{
    public class Gravity
    {
        public const int NO_GRAVITY = 0x0000;
        public const int AXIS_SPECIFIED = 0x0001;
        public const int AXIS_PULL_BEFORE = 0x0002;
        public const int AXIS_PULL_AFTER = 0x0004;
        public const int AXIS_CLIP = 0x0008;
        public const int AXIS_X_SHIFT = 0;
        public const int AXIS_Y_SHIFT = 4;
        public const int TOP = (AXIS_PULL_BEFORE | AXIS_SPECIFIED) << AXIS_Y_SHIFT;
        public const int BOTTOM = (AXIS_PULL_AFTER | AXIS_SPECIFIED) << AXIS_Y_SHIFT;
        public const int LEFT = (AXIS_PULL_BEFORE | AXIS_SPECIFIED) << AXIS_X_SHIFT;
        public const int RIGHT = (AXIS_PULL_AFTER | AXIS_SPECIFIED) << AXIS_X_SHIFT;
        public const int CENTER_VERTICAL = AXIS_SPECIFIED << AXIS_Y_SHIFT;
        public const int FILL_VERTICAL = TOP | BOTTOM;
        public const int CENTER_HORIZONTAL = AXIS_SPECIFIED << AXIS_X_SHIFT;
        public const int FILL_HORIZONTAL = LEFT | RIGHT;
        public const int CENTER = CENTER_VERTICAL | CENTER_HORIZONTAL;
        public const int FILL = FILL_VERTICAL | FILL_HORIZONTAL;
        public const int CLIP_VERTICAL = AXIS_CLIP << AXIS_Y_SHIFT;
        public const int CLIP_HORIZONTAL = AXIS_CLIP << AXIS_X_SHIFT;
        public const int RELATIVE_LAYOUT_DIRECTION = 0x00800000;
        public const int HORIZONTAL_GRAVITY_MASK = (AXIS_SPECIFIED | AXIS_PULL_BEFORE | AXIS_PULL_AFTER) << AXIS_X_SHIFT;
        public const int VERTICAL_GRAVITY_MASK = (AXIS_SPECIFIED | AXIS_PULL_BEFORE | AXIS_PULL_AFTER) << AXIS_Y_SHIFT;
        public const int DISPLAY_CLIP_VERTICAL = 0x10000000;
        public const int DISPLAY_CLIP_HORIZONTAL = 0x01000000;
        public const int START = RELATIVE_LAYOUT_DIRECTION | LEFT;
        public const int END = RELATIVE_LAYOUT_DIRECTION | RIGHT;
        public const int RELATIVE_HORIZONTAL_GRAVITY_MASK = START | END;


    }
}
