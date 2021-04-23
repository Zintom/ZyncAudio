using System.Drawing;
using System.Windows.Forms;
using ZyncAudio.Extensions;

namespace ZyncAudio
{
    public class TintableButton : Button
    {
        public override Image BackgroundImage
        {
            get => base.BackgroundImage;
            set
            {
                base.BackgroundImage = value;
                ApplyTint();
            }
        }

        private bool _checked;
        public bool Checked
        {
            get => _checked;
            set
            {
                _checked = value;
                ApplyTint();
            }
        }

        private Color _checkedTint = Color.Empty;
        public Color CheckedTint
        {
            get => _checkedTint;
            set
            {
                _checkedTint = value;
                ApplyTint();
            }
        }

        private Color _tint = Color.Empty;
        public Color Tint
        {
            get => _tint;
            set
            {
                _tint = value;
                ApplyTint();
            }
        }

        private RotateFlipType _flipEffect = RotateFlipType.RotateNoneFlipNone;
        public RotateFlipType FlipEffect
        {
            get => _flipEffect;
            set
            {
                Bitmap flipped = (Bitmap)base.BackgroundImage;
                flipped.RotateFlip(value);
                _flipEffect = value;
            }
        }

        /// <summary>
        /// Applies the correct tint to the <see cref="BackgroundImage"/> depending on the <see cref="Checked"/> state.
        /// </summary>
        private void ApplyTint()
        {
            if (Checked)
            {
                if (CheckedTint == Color.Empty) return;
                ((Bitmap)base.BackgroundImage).Tint(CheckedTint);
            }
            else
            {
                if (Tint == Color.Empty) return;
                ((Bitmap)base.BackgroundImage).Tint(Tint);
            }

            Invalidate();
        }

    }
}
