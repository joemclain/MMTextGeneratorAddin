using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

using MahApps.Metro.Controls;
using MahApps.Metro.Controls.Dialogs;
using MarkdownMonster;
using MarkdownMonster.AddIns;
using MarkdownMonster.Windows;
using Westwind.Utilities;

using LoremIpsum;



using TextGenerator;

namespace TextGenerator.Forms {
	
	/// <summary>
	/// Interaction logic for TextGenForm.xaml
	/// </summary>
	
	public partial class TextGenForm : MetroWindow {

		private readonly Action _closeHandler;
		private readonly TextGeneratorAddin _mmAddin;

		private readonly AddinModel _model;
		private readonly DialogModel _viewData;

		/////////////////////////////////////////////////////////////////////////////

		public TextGenForm( AddinModel model, Action closeHandler )
		{
			// ******
			_model = model ?? throw new ArgumentNullException( nameof( model ) );
			_mmAddin = model.Addin;
			_viewData = model.Configuration.Data;

			// ******
			_closeHandler = closeHandler ?? throw new ArgumentNullException( nameof( closeHandler ) );

			// ******
			DataContext = _viewData;
			InitializeComponent();

			// ******
			//
			// set parent window so we'll appear in front of, and stay above it
			//
			Owner = _model.AppModel.Window;
			//
			// MahApps.Metro
			//
			mmApp.SetThemeWindowOverride( this );

			// ******
			//
			// initialize radio buttons
			//
			SetInsertType();
		}


		/////////////////////////////////////////////////////////////////////////////

		private void SetInsertType()
		{
			switch( _viewData.LastInsertType ) {
				case DialogModel.InsertType.Words:
					Words.IsChecked = true;
					break;

				case DialogModel.InsertType.Sentences:
					Sentences.IsChecked = true;
					break;

				default:// Paragraphs
					Paragraphs.IsChecked = true;
					break;
			}
		}


		/////////////////////////////////////////////////////////////////////////////

		private void UpdateConfig()
		{
			AddinConfiguration.Current.Write();
		}


		/////////////////////////////////////////////////////////////////////////////

		private void Form_Loaded( object sender, RoutedEventArgs e )
		{
			//var dc = DataContext;
		}


		/////////////////////////////////////////////////////////////////////////////

		private void Form_Closing( object sender, System.ComponentModel.CancelEventArgs e )
		{
			_closeHandler();
			UpdateConfig();
		}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// One of the insert type radio buttons has been clicked
		///
		/// Note: we could wrap the radio buttons with a ??? control and place
		/// the handler on that control rather than all three radio buttons
		/// 
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void InsertTypeChecked( object sender, RoutedEventArgs e )
		{
			if( sender.Equals( Words ) ) {
				_viewData.LastInsertType = DialogModel.InsertType.Words;
			}
			else if( sender.Equals( Sentences ) ) {
				_viewData.LastInsertType = DialogModel.InsertType.Sentences;
			}
			else {
				//
				// must equal Paragraphs
				//
				_viewData.LastInsertType = DialogModel.InsertType.Paragraphs;
			}
		}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Text box value has changed - each time any change is made
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void TBCount_TextChanged( object sender, TextChangedEventArgs e )
		{

		}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Handle each key typed in the text box, only letting numeric and delete
		/// keys to be handled
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>
		
		private void TBCount_KeyDown( object sender, KeyEventArgs e )
		{
			// ******
			switch( e.Key ) {
				case Key.Back:
				case Key.Delete:

				case Key.D0:
				case Key.D1:
				case Key.D2:
				case Key.D3:
				case Key.D4:
				case Key.D5:
				case Key.D6:
				case Key.D7:
				case Key.D8:
				case Key.D9:

				case Key.NumPad0:
				case Key.NumPad1:
				case Key.NumPad2:
				case Key.NumPad3:
				case Key.NumPad4:
				case Key.NumPad5:
				case Key.NumPad6:
				case Key.NumPad7:
				case Key.NumPad8:
				case Key.NumPad9:
					//
					// let the control handle the keys
					//
					e.Handled = false;
					break;

				default:
					//
					// text box won't receive the keys
					//
					e.Handled = true;
					break;
			}
		}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Range check value in text box.
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private void TBCount_LostFocus( object sender, RoutedEventArgs e )
		{
			// ******
			var tb = sender as TextBox;

			if( int.TryParse( tb.Text, NumberStyles.Integer, null, out int i ) ) {
				if( i < 1 ) {
					tb.Text = "1";
				}
				else if( i > 99 ) {
					tb.Text = "99";
				}

				return;
			}

			// ******
			tb.Text = "1";
		}


		/////////////////////////////////////////////////////////////////////////////
		/// <summary>
		/// Insert button has been clicked
		/// </summary>
		/// <param name="sender"></param>
		/// <param name="e"></param>

		private async void InsertButton( object sender, RoutedEventArgs e )
		{
			// ******
			UpdateConfig();

			// ******
			var tg = _model.LoremIpsum;

			const int MIN_SENTENCES_PARAGRAPH = 3;
			const int MAX_SENTENCES_PARAGRAPH = 18;

			var templates = new SentenceTemplate [] {
				new SentenceTemplate( 1, 15),
				new SentenceTemplate( 6, 9),
				new SentenceTemplate( 11, 45),
				new SentenceTemplate( 3, 11),
				new SentenceTemplate( 9, 18),
			};

			string text = string.Empty;

			switch( _viewData.LastInsertType ) {
				case DialogModel.InsertType.Words:
					text = tg.GetWords( _viewData.CountItems );
					if( _viewData.CapitalizeWords ) {
						text = local_SetCaps( text );
					}
					break;

				case DialogModel.InsertType.Sentences:
					text = tg.GetSentences( _viewData.CountItems, templates );
					break;

				default:// Paragraphs
					var minMaxSentencesInParagraph = new IntRange( MIN_SENTENCES_PARAGRAPH, MAX_SENTENCES_PARAGRAPH );
					text = tg.GetParagraphs( _viewData.CountItems, minMaxSentencesInParagraph, templates );
					break;
			}

			await _mmAddin.SetSelection( text );

			// show status message that goes away after 4 secs
			_mmAddin.ShowStatus( "Inserted generated text...", 4000 );

			// Set focus back to editor
			await _mmAddin.SetEditorFocus();



			// ******
			string local_SetCaps( string str )
			{
				var sb = new StringBuilder { };
				var lines = str.Split( ' ' );
				foreach( var line in lines ) {
					if( sb.Length > 0 ) {
						sb.Append( ' ' );
					}

					sb.Append( char.ToUpper( line [ 0 ] ) );
					sb.Append( line.Substring( 1 ) );
				}

				return sb.ToString();
			}

		}


		/////////////////////////////////////////////////////////////////////////////

		private async void InsertAndClose( object sender, RoutedEventArgs e )
		{
			InsertButton( sender, e );
			this.Close();
		}

	}
}
