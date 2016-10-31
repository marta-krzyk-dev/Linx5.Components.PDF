using System;

namespace Twenty57.Linx.Components.Pdf.Helpers
{
	internal class VisibleDependency
	{
		private bool? visibleResult;
		private Func<bool> isVisible;

		public VisibleDependency(Func<bool> isVisible)
		{
			if (isVisible == null)
				throw new ArgumentNullException(nameof(isVisible));

			this.isVisible = isVisible;
			this.visibleResult = null;
		}

		public bool Visible
		{
			get
			{
				if (!this.visibleResult.HasValue)
					this.visibleResult = this.isVisible();
				return this.visibleResult.Value;
			}
		}

		public void Refresh()
		{
			this.visibleResult = null;
			VisibleChanged?.Invoke(Visible);
		}

		public delegate void OnVisibleChanged(bool visible);
		public event OnVisibleChanged VisibleChanged;
	}
}
