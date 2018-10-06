using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace DiscordRichPresence.Controls
{

	/// <summary>
	/// Interaction logic for ValidatableTextBox.xaml
	/// </summary>
	public partial class ValidatableTextBox : UserControl
	{
		internal Object Value { get; private set; }
		internal delegate Object Validator(ValidatableTextBox box, String value);
		private Validator _validator = (_, __) => null;

		private Brush _textHint;

		[Browsable(true)]
		[Bindable(true)]
		[Category("Appearance")]
		public String TextHint
		{
			get => ((this._textHint as VisualBrush).Visual as Label).Content as String;
			set
			{
				this._textHint = this.InputTextBox.Background = new VisualBrush()
				{
					AlignmentX = AlignmentX.Left,
					AlignmentY = AlignmentY.Center,
					Stretch = Stretch.None,
					Visual = new Label()
					{
						Content = value,
						Foreground = Brushes.LightGray
					}
				};
			}
		}

		public ValidatableTextBox() => this.InitializeComponent();

		internal void SetValidator(Validator validator) => this._validator = validator;

		private void OnTextBoxTextChanged(Object sender, EventArgs e)
		{
			TextBox textBox = (TextBox)sender;
			Object result = this._validator(this, textBox.Text);

			if (String.IsNullOrEmpty(textBox.Text))
				textBox.Background = this._textHint;


			// This is incredible bad, yes
			if (result == this)
			{
				this.Value = null;
				textBox.BorderBrush = Brushes.Red;
			}
			else
			{
				this.Value = result;
				textBox.BorderBrush = Brushes.Green;
			}
		}

		private void OnIsKeyboardFocusedChanged(Object sender, DependencyPropertyChangedEventArgs e)
		{
			TextBox textBox = (TextBox)sender;

			if (e.NewValue.Equals(true))
				textBox.Background = Brushes.White;
			else if (String.IsNullOrEmpty(textBox.Text))
				textBox.Background = this._textHint;
		}
	}
}
