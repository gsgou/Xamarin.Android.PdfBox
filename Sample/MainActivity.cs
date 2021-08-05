using System;
using System.Collections.Generic;
using System.IO;

using Android.App;
using Android.Content.Res;
using Android.Graphics;
using Android.OS;
using Android.Runtime;
using Android.Util;
using Android.Views;
using Android.Widget;

using AndroidX.AppCompat.App;

using File = Java.IO.File;
using Integer = Java.Lang.Integer;
using IOException = Java.IO.IOException;

using PDDocument = Com.Tom_roush.Pdfbox.Pdmodel.PDDocument;
using PDPage = Com.Tom_roush.Pdfbox.Pdmodel.PDPage;
using PDPageContentStream = Com.Tom_roush.Pdfbox.Pdmodel.PDPageContentStream;
using PDFont = Com.Tom_roush.Pdfbox.Pdmodel.Font.PDFont;
using PDType1Font = Com.Tom_roush.Pdfbox.Pdmodel.Font.PDType1Font;
using PDType0Font = Com.Tom_roush.Pdfbox.Pdmodel.Font.PDType0Font;
using JPEGFactory = Com.Tom_roush.Pdfbox.Pdmodel.Graphics.Image.JPEGFactory;
using LosslessFactory = Com.Tom_roush.Pdfbox.Pdmodel.Graphics.Image.LosslessFactory;
using PDCheckBox = Com.Tom_roush.Pdfbox.Pdmodel.Interactive.Form.PDCheckBox;
using PDComboBox = Com.Tom_roush.Pdfbox.Pdmodel.Interactive.Form.PDComboBox;
using PDListBox = Com.Tom_roush.Pdfbox.Pdmodel.Interactive.Form.PDListBox;
using PDRadioButton = Com.Tom_roush.Pdfbox.Pdmodel.Interactive.Form.PDRadioButton;
using PDTextField = Com.Tom_roush.Pdfbox.Pdmodel.Interactive.Form.PDTextField;
using PDFRenderer = Com.Tom_roush.Pdfbox.Rendering.PDFRenderer;
using PDFTextStripper = Com.Tom_roush.Pdfbox.Text.PDFTextStripper;
using PDFBoxResourceLoader = Com.Tom_roush.Pdfbox.Util.PDFBoxResourceLoader;
using ImageType = Com.Tom_roush.Pdfbox.Rendering.ImageType;

namespace PdfBoxAndroidSample
{
	[Activity(Label = "@string/app_name", Theme = "@style/AppTheme", MainLauncher = true)]
	public class MainActivity : AppCompatActivity
	{
		File root;
		AssetManager assetManager;
		Bitmap pageImage;
		TextView tv;

		protected override void OnCreate(Bundle savedInstanceState)
		{
			base.OnCreate(savedInstanceState);

			Xamarin.Essentials.Platform.Init(this, savedInstanceState);

			// Set our view from the "main" layout resource
			SetContentView(Resource.Layout.activity_main);

			var buttonCreate = FindViewById<Button>(Resource.Id.buttonCreate);
			buttonCreate.Click += createPdf;

			var buttonRender = FindViewById<Button>(Resource.Id.buttonRender);
			buttonRender.Click += renderFile;

			var buttonFillForm = FindViewById<Button>(Resource.Id.buttonFillForm);
			buttonFillForm.Click += fillForm;

			var buttonStripText = FindViewById<Button>(Resource.Id.buttonStripText);
			buttonStripText.Click += stripText;
		}

		public override void OnRequestPermissionsResult(int requestCode, string[] permissions, [GeneratedEnum] Android.Content.PM.Permission[] grantResults)
		{
			Xamarin.Essentials.Platform.OnRequestPermissionsResult(requestCode, permissions, grantResults);

			base.OnRequestPermissionsResult(requestCode, permissions, grantResults);
		}

		protected override void OnStart()
		{
			base.OnStart();

			Setup();
		}

		public override bool OnPrepareOptionsMenu(IMenu menu)
		{
			//MenuInflater.Inflate(Resource.Menu.menu_main, menu);
			return base.OnPrepareOptionsMenu(menu);
		}

		// Initializes variables used for convenience
		private void Setup()
		{
			// Enable Android-style asset loading (highly recommended)
			PDFBoxResourceLoader.Init(Application.Context);
			// Find the root of the internal storage.
			root = Application.Context.CacheDir;
			assetManager = this.Assets;
			tv = (TextView)FindViewById(Resource.Id.statusTextView);
		}

        // Creates a new PDF from scratch and saves it to a file
		public void createPdf(object sender, EventArgs e)
		{
			using (var document = new PDDocument())
			using (var page = new PDPage())
			{
				document.AddPage(page);

				// Create a new font object selecting one of the PDF base fonts
				using (PDFont font = PDType1Font.Helvetica)
				{
					// Or a custom font
					//try
					//{
					//	// Replace MyFontFile with the path to the asset font you'd like to use.
					//	// Or use LiberationSans "com/tom_roush/pdfbox/resources/ttf/LiberationSans-Regular.ttf"
					//	font = PDType0Font.Load(document, assetManager.Open("MyFontFile.ttf"));
					//}
					//catch (IOException ex)
					//{
					//	Log.Error("PdfBox-Android-Sample", "Could not load font", ex);
					//}

					try
					{
						// Define a content stream for adding to the PDF
						using (var contentStream = new PDPageContentStream(document, page))
						{
							// Write Hello World in blue text
							contentStream.BeginText();
							contentStream.SetNonStrokingColor(15, 38, 192);
							contentStream.SetFont(font, 12);
							contentStream.NewLineAtOffset(100, 700);
							contentStream.ShowText("Hello World");
							contentStream.EndText();

							// Load in the images
							var inStream = assetManager.Open("falcon.jpg");
							var alphaStream = assetManager.Open("trans.png");

							// Draw a green rectangle
							contentStream.AddRect(5, 500, 100, 100);
							contentStream.SetNonStrokingColor(0, 255, 125);
							contentStream.Fill();

							// Draw the falcon base image
							using (var ximage = JPEGFactory.CreateFromStream(document, inStream))
							{
								contentStream.DrawImage(ximage, 20, 20);
							}

							// Draw the red overlay image
							using (var alphaImage = BitmapFactory.DecodeStream(alphaStream))
							using (var alphaXimage = LosslessFactory.CreateFromImage(document, alphaImage))
							{
								contentStream.DrawImage(alphaXimage, 20, 20);
							}

							// Make sure that the content stream is closed:
							contentStream.Close();

							// Save the final pdf document to a file
							string path = root.AbsolutePath + "/Created.pdf";
							document.Save(path);
							document.Close();
							tv.Text = "Successfully wrote PDF to " + path;
						}
					}
					catch (IOException ex)
					{
						Log.Error("PdfBox-Android-Sample", "Exception thrown while creating PDF", ex);
					}
				}
			}
		}

		// Loads an existing PDF and renders it to a Bitmap
		public void renderFile(object sender, EventArgs e)
		{
			// Render the page and save it to an image file
			try
			{
				// Load in an already created PDF
				using (var document = PDDocument.Load(assetManager.Open("Created.pdf")))
				// Create a renderer for the document
				using (var renderer = new PDFRenderer(document))
				{
					// Render the image to an RGB Bitmap
					pageImage = renderer.RenderImage(0, 1, ImageType.Rgb);

					// Save the render result to an image
					string filePath = root.AbsolutePath + "/render.jpg";
					using (var stream = new FileStream(filePath, FileMode.Create))
					{
						pageImage.Compress(Bitmap.CompressFormat.Jpeg, 100, stream);
					}

					tv.Text = "Successfully rendered image to " + filePath;
					// Optional: display the render result on screen
					displayRenderedImage();
				}
			}
			catch (IOException ex)
			{
				Log.Error("PdfBox-Android-Sample", "Exception thrown while rendering file", ex);
			}
		}

		// Fills in a PDF form and saves the result
		public void fillForm(object sender, EventArgs e)
		{
			try
			{
				// Load the document and get the AcroForm
				using (var document = PDDocument.Load(assetManager.Open("FormTest.pdf")))
				using (var docCatalog = document.DocumentCatalog)
				using (var acroForm = docCatalog.AcroForm)
				{
					// Fill the text field
					using (var field = (PDTextField)acroForm.GetField("TextField"))
					{
						field.SetValue("Filled Text Field");
						// Optional: don't allow this field to be edited
						field.ReadOnly = true;
					}

					using (var checkbox = acroForm.GetField("Checkbox"))
					{
						((PDCheckBox)checkbox).Check();
					}

					using (var radio = acroForm.GetField("Radio"))
					{
						((PDRadioButton)radio).SetValue("Second");
					}

					// TODO: Use List<int>
					using (var listbox = acroForm.GetField("ListBox"))
					{
						List<Integer> listValues = new List<Integer>();
						listValues.Add(Integer.ValueOf(1.ToString()));
						listValues.Add(Integer.ValueOf(2.ToString()));
						((PDListBox)listbox).SelectedOptionsIndex = listValues;
					}

					/*
					// TODO: Fix Exception
					using (var dropdown = acroForm.GetField("Dropdown"))
					{
						IList<string> list = new List<string>();
						list.Add("Hello");
						((PDComboBox)dropdown).Value = list;
					}
					*/

					string path = root.AbsolutePath + "/FilledForm.pdf";
					tv.Text = "Saved filled form to " + path;
					document.Save(path);
					document.Close();
				}
			}
			catch (IOException ex)
			{
				Log.Error("PdfBox-Android-Sample", "Exception thrown while filling form fields", ex);
			}
		}

		// Strips the text from a PDF and displays the text on screen
		public void stripText(object sender, EventArgs e)
		{
			string parsedText = null;
			PDDocument document = null;
			try
			{
				document = PDDocument.Load(assetManager.Open("Hello.pdf"));
			}
			catch (IOException ex)
			{
				Log.Error("PdfBox-Android-Sample", "Exception thrown while loading document to strip", ex);
			}

			try
			{
				using (var pdfStripper = new PDFTextStripper())
				{
					pdfStripper.StartPage = 0;
					pdfStripper.EndPage = 1;
					parsedText = "Parsed text: " + pdfStripper.GetText(document);
				}
			}
			catch (IOException ex)
			{
				Log.Error("PdfBox-Android-Sample", "Exception thrown while stripping text", ex);
			}
			finally
			{
				try
				{
					if (document != null)
					{
						document.Close();
						document.Dispose();
					}
				}
				catch (IOException ex)
				{
					Log.Error("PdfBox-Android-Sample", "Exception thrown while closing document", ex);
				}
			}
			tv.Text = parsedText;
		}

		// Helper method for drawing the result of renderFile() on screen
		private void displayRenderedImage()
		{
			RunOnUiThread(() => {
				var imageView = (ImageView)FindViewById(Resource.Id.renderedImageView);
				imageView.SetImageBitmap(pageImage);
			});
		}
	}
}