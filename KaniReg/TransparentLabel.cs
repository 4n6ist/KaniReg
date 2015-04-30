using System;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Runtime.InteropServices;
using System.Windows.Forms;

// 結局使わなかった

namespace KaniReg
{
	public class TransparentLabel : System.Windows.Forms.Label
	{
		public TransparentLabel()
		{
			// 適用されていない場合は SupportsTransparentBackColor を true にする
			// this.SetStyle(ControlStyles.SupportsTransparentBackColor, true); 
		}

		private bool _AllowTransparency;
		/// <summary>
		/// 背景が透明なとき背面のコントロールを描画するかどうかを示す値取得または設定します。
		/// </summary>
		[Category("表示")]
		[DefaultValue(false)]
		[Description("背景が透明なとき背面のコントロールが表示されるどうかを示します。")]
		public bool AllowTransparency
		{
			get
			{
				return _AllowTransparency;
			}
			set
			{
				if (_AllowTransparency == value)
				{
					return;
				}

				_AllowTransparency = value;

				this.Invalidate();
			}
		}

		protected override void OnPaintBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
			// 背面のコントロールを描画しない Or 背景色が不透明なので背面のコントロールを描画する必要なし
			if ((this.AllowTransparency == false) || (this.BackColor.A == 255))
			{
				base.OnPaintBackground(pevent);
				return;
			}

			// 背面のコントロールを描画
			this.DrawParentWithBackControl(pevent);

			// 背景を描画
			this.DrawBackground(pevent);
		}

		/// <summary>
		/// コントロールの背景を描画します。
		/// </summary>
		/// <param name="pevent">描画先のコントロールに関連する情報</param>
		private void DrawBackground(System.Windows.Forms.PaintEventArgs pevent)
		{
			// 背景色
			using (SolidBrush sb = new SolidBrush(this.BackColor))
			{
				pevent.Graphics.FillRectangle(sb, this.ClientRectangle);
			}

			// 背景画像
			if (this.BackgroundImage != null)
			{
				this.DrawBackgroundImage(pevent.Graphics, this.BackgroundImage, this.BackgroundImageLayout);
			}
		}

		/// <summary>
		/// コントロールの背景画像を描画します。
		/// </summary>
		/// <param name="g">描画に使用するグラフィックス オブジェクト</param>
		/// <param name="img">描画する画像</param>
		/// <param name="layout">画像のレイアウト</param>
		private void DrawBackgroundImage(Graphics g, Image img, ImageLayout layout)
		{
			Size imgSize = img.Size;

			switch (layout)
			{
				case ImageLayout.None:
					g.DrawImage(img, 0, 0, imgSize.Width, imgSize.Height);

					break;
				case ImageLayout.Tile:
					int xCount = Convert.ToInt32(Math.Ceiling((double)this.ClientRectangle.Width / (double)imgSize.Width));
					int yCount = Convert.ToInt32(Math.Ceiling((double)this.ClientRectangle.Height / (double)imgSize.Height));
					for (int x = 0; x <= xCount - 1; x++)
					{
						for (int y = 0; y <= yCount - 1; y++)
						{
							g.DrawImage(img, imgSize.Width * x, imgSize.Height * y, imgSize.Width, imgSize.Height);
						}
					}

					break;
				case ImageLayout.Center:
					{
						int x = 0;
						if (this.ClientRectangle.Width > imgSize.Width)
						{
							x = (int)Math.Floor((double)(this.ClientRectangle.Width - imgSize.Width) / 2.0);
						}
						int y = 0;
						if (this.ClientRectangle.Height > imgSize.Height)
						{
							y = (int)Math.Floor((double)(this.ClientRectangle.Height - imgSize.Height) / 2.0);
						}
						g.DrawImage(img, x, y, imgSize.Width, imgSize.Height);

						break;
					}
				case ImageLayout.Stretch:
					g.DrawImage(img, 0, 0, this.ClientRectangle.Width, this.ClientRectangle.Height);

					break;
				case ImageLayout.Zoom:
					{
						double xRatio = (double)this.ClientRectangle.Width / (double)imgSize.Width;
						double yRatio = (double)this.ClientRectangle.Height / (double)imgSize.Height;
						double minRatio = Math.Min(xRatio, yRatio);

						Size zoomSize = new Size(Convert.ToInt32(Math.Ceiling(imgSize.Width * minRatio)), Convert.ToInt32(Math.Ceiling(imgSize.Height * minRatio)));

						int x = 0;
						if (this.ClientRectangle.Width > zoomSize.Width)
						{
							x = (int)Math.Floor((double)(this.ClientRectangle.Width - zoomSize.Width) / 2.0);
						}
						int y = 0;
						if (this.ClientRectangle.Height > zoomSize.Height)
						{
							y = (int)Math.Floor((double)(this.ClientRectangle.Height - zoomSize.Height) / 2.0);
						}
						g.DrawImage(img, x, y, zoomSize.Width, zoomSize.Height);

						break;
					}
			}
		}

		/// <summary>
		/// 親コントロールと背面にあるコントロールを描画します。
		/// </summary>
		/// <param name="pevent">描画先のコントロールに関連する情報</param>
		private void DrawParentWithBackControl(System.Windows.Forms.PaintEventArgs pevent)
		{
			// 親コントロールを描画
			this.DrawParentControl(this.Parent, pevent);

			// 親コントロールとの間のコントロールを親側から描画
			for (int i = this.Parent.Controls.Count - 1; i >= 0; i--)
			{
				Control c = this.Parent.Controls[i];
				if (c == this)
				{
					break;
				}
				if (this.Bounds.IntersectsWith(c.Bounds) == false)
				{
					continue;
				}

				this.DrawBackControl(c, pevent);
			}
		}

		/// <summary>
		/// 親コントロールを描画します。
		/// </summary>
		/// <param name="c">親コントロール</param>
		/// <param name="pevent">描画先のコントロールに関連する情報</param>
		private void DrawParentControl(Control c, System.Windows.Forms.PaintEventArgs pevent)
		{
			using (Bitmap bmp = new Bitmap(c.Width, c.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
			{
				using (Graphics g = Graphics.FromImage(bmp))
				{
					using (PaintEventArgs p = new PaintEventArgs(g, c.ClientRectangle))
					{
						this.InvokePaintBackground(c, p);
						this.InvokePaint(c, p);
					}
				}

				int offsetX = this.Left + (int)Math.Floor((double)(this.Bounds.Width - this.ClientRectangle.Width) / 2.0);
				int offsetY = this.Top + (int)Math.Floor((double)(this.Bounds.Height - this.ClientRectangle.Height) / 2.0);
				pevent.Graphics.DrawImage(bmp, this.ClientRectangle, new Rectangle(offsetX, offsetY, this.ClientRectangle.Width, this.ClientRectangle.Height), GraphicsUnit.Pixel);
			}
		}

		/// <summary>
		/// 背面のコントロールを描画します。
		/// </summary>
		/// <param name="c">背面のコントロール</param>
		/// <param name="pevent">描画先のコントロールに関連する情報</param>
		private void DrawBackControl(Control c, System.Windows.Forms.PaintEventArgs pevent)
		{
			using (Bitmap bmp = new Bitmap(c.Width, c.Height, System.Drawing.Imaging.PixelFormat.Format32bppArgb))
			{
				c.DrawToBitmap(bmp, new Rectangle(0, 0, c.Width, c.Height));

				int offsetX = (c.Left - this.Left) - (int)Math.Floor((double)(this.Bounds.Width - this.ClientRectangle.Width) / 2.0);
				int offsetY = (c.Top - this.Top) - (int)Math.Floor((double)(this.Bounds.Height - this.ClientRectangle.Height) / 2.0);
				pevent.Graphics.DrawImage(bmp, offsetX, offsetY, c.Width, c.Height);
			}
		}
	}

	/*class TransparentLabel : System.Windows.Forms.Label
	{
		private void UpdateRegion()
		{
			Region r = new Region(this.ClientRectangle);
			int w = this.ClientSize.Width;
			int h = this.ClientSize.Height;

			// クライアント領域と同じ大きさの Bitmap クラスを生成します。
			Bitmap foreBitmap = new Bitmap(w, h, PixelFormat.Format32bppArgb);

			// 文字列などの背景以外の部分を描画します。
			using (Graphics g = Graphics.FromImage(foreBitmap))
			{
				using (SolidBrush sb = new SolidBrush(this.ForeColor))
				{
					Rectangle rect = new Rectangle(
						this.Padding.Left,
						this.Padding.Top,
						this.ClientRectangle.Width - this.Padding.Left - this.Padding.Right,
						this.ClientRectangle.Height - this.Padding.Top - this.Padding.Bottom
					);

					g.DrawString(this.Text, this.Font, sb, rect);
				}
			}

			// できた Bitmap クラスからピクセルの色情報を取得します。
			BitmapData bd = foreBitmap.LockBits(this.ClientRectangle, ImageLockMode.ReadWrite, PixelFormat.Format32bppArgb);
			int stride = bd.Stride;
			int bytes = stride * h;
			byte[] bgraValues = new byte[bytes];
			Marshal.Copy(bd.Scan0, bgraValues, 0, bytes);
			foreBitmap.UnlockBits(bd);
			foreBitmap.Dispose();

			// 描画された部分だけの領域を作成します。
			int line = 0;
			for (int y = 0; y < h; y++)
			{
				line = stride * y;
				for (int x = 0; x < w; x++)
				{
					// アルファ値が 0 は背景
					if (bgraValues[line + x * 4 + 3] == 0)
					{
						r.Exclude(new Rectangle(x, y, 1, 1));
					}
				}
			}

			// Region に文字列の領域を設定します。
			this.Region = r;
		}

		protected override void OnTextChanged(EventArgs e)
		{
			if ((this.DesignMode == false) && (this.BackColor == Color.Transparent))
			{
				this.UpdateRegion();
			}
			base.OnTextChanged(e);
		}

		protected override void OnSizeChanged(EventArgs e)
		{
			if ((this.DesignMode == false) && (this.BackColor == Color.Transparent))
			{
				this.UpdateRegion();
			}
			base.OnSizeChanged(e);
		}

		protected override void OnPaddingChanged(EventArgs e)
		{
			if ((this.DesignMode == false) && (this.BackColor == Color.Transparent))
			{
				this.UpdateRegion();
			}
			base.OnPaddingChanged(e);
		}

		protected override void OnPaintBackground(PaintEventArgs pevent)
		{
			if (this.BackColor == Color.Transparent)
			{
				if (this.DesignMode == true)
				{
					// デザイン時は通常の描画
					base.OnPaintBackground(pevent);
				}
				else
				{
					// 文字列を塗りつぶす
					using (SolidBrush sb = new SolidBrush(this.ForeColor))
					{
						pevent.Graphics.FillRectangle(sb, this.ClientRectangle);
					}
				}
			}
			else
			{
				base.OnPaintBackground(pevent);
			}
		}

		protected override void OnPaint(PaintEventArgs e)
		{
			if (this.BackColor == Color.Transparent)
			{
				if (this.DesignMode == true)
				{
					// デザイン時は通常の描画
					base.OnPaint(e);
				}
			}
			else
			{
				base.OnPaint(e);
			}
		}
	}*/
}
