using MarkdownMonster.AddIns;

using Westwind.Utilities.Configuration;

namespace TextGenerator {

	/////////////////////////////////////////////////////////////////////////////

	public class AddinConfiguration : BaseAddinConfiguration<AddinConfiguration> {

		// ******
		public DialogModel Data { get; set; }

		// ******
#if DEBUG
		/// <summary>
		/// Keyboard shortcut used during debugging; currently not available
		/// for a release build
		/// </summary>
		public string KeyboardShortcut
		{
			get { return _keyboardShortcut; }
			set {
				if( _keyboardShortcut == value )
					return;
				_keyboardShortcut = value;
				OnPropertyChanged( nameof( KeyboardShortcut ) );
			}
		}

		private string _keyboardShortcut = "Alt+a";
#endif

		/////////////////////////////////////////////////////////////////////////////

		public AddinConfiguration()
		{
			ConfigurationFilename = "TextGeneratorAddin.json";
		}


		/////////////////////////////////////////////////////////////////////////////

		protected override void OnInitialize( IConfigurationProvider provider, string sectionName, object configData )
		{
			base.OnInitialize( provider, sectionName, configData );
			//
			// Data will be not be initialized from storage if no Data
			// entry has been saved
			//
			if( null == Data ) {
				Data = new DialogModel { };
			}
		}


	}

	/////////////////////////////////////////////////////////////////////////////

	public class DialogModel {

		public enum InsertType {
			Words,
			Sentences,
			Paragraphs
		};

		public InsertType LastInsertType { get; set; } = InsertType.Words;
		public int CountItems { get; set; } = 1;
		public bool CapitalizeWords { get; set; } = false;

	}




}