using System;
using System.Drawing;
using System.Windows.Forms;
using ZyncAudio.Extensions;

namespace ZyncAudio.CustomControls
{
    public class TintableButton : Button, IDisposable
    {
        /// <summary>
        /// This is so that we may restore an untinted image if the control goes from
        /// being Disabled > Enabled and NO tint or checked tint is applied.
        /// </summary>
        private Bitmap? _originalImage;

        public override Image BackgroundImage
        {
            get
            {
                if (UseOriginalBackgroundImage && _originalImage != null)
                {
                    return _originalImage;
                }
                else
                {
                    return base.BackgroundImage;
                }
            }
            set
            {
                if (value == null)
                {
                    base.BackgroundImage = null;
                    return;
                }

                _originalImage = (Bitmap)value.Clone();
                base.BackgroundImage = value;
                ApplyTint();
            }
        }

        /// <summary>
        /// The only time we should the original background image
        /// is if the control isn't <b>Disabled</b> (otherwise it would be grey tint) and has <i>NO</i> <b>Tint</b> or <b>Checked Tint</b> for its enabled state.
        /// </summary>
        private bool UseOriginalBackgroundImage
        {
            get
            {
                return (Checked && CheckedTint == Color.Empty) || (!Checked && Tint == Color.Empty) && Enabled && base.BackgroundImage != null;
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
        private bool _disposedValue;

        public RotateFlipType FlipEffect
        {
            get => _flipEffect;
            set
            {
                if (base.BackgroundImage == null) return;

                Bitmap flipped = (Bitmap)base.BackgroundImage;
                flipped.RotateFlip(value);
                _flipEffect = value;
            }
        }

        protected override void OnEnabledChanged(EventArgs e)
        {
            ApplyTint();
            base.OnEnabledChanged(e);
        }

        /// <summary>
        /// Applies the correct tint to the <see cref="BackgroundImage"/> depending on the <see cref="Checked"/> state.
        /// </summary>
        private void ApplyTint()
        {
            if (base.BackgroundImage == null) return;

            if (!Enabled)
            {
                ((Bitmap)base.BackgroundImage).Tint(Color.Gray);
            }
            else if (Checked)
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

        protected override void Dispose(bool disposing)
        {
            if (!_disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    _originalImage?.Dispose();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                _originalImage = null;
                _disposedValue = true;
            }

            base.Dispose(disposing);
        }

    }
}
