using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Threading.Tasks;

namespace just_one_small_task;

public partial class GamePage : ContentPage
{
	public int PlayerCount;

	// Cards will be loaded lazily from Resources/Raw/cards.json
	private List<string>? _cards;
	
	private int _currentCardIndex = -1;
	private readonly Random _rng = new Random();

	public GamePage()
	{
		InitializeComponent();

		var picker = this.FindByName<Picker>("PlayerCountPicker");
		if (picker != null)
		{
			for (int i = 2; i <= 29; i++)
			{
				picker.Items.Add(i.ToString());
			}

			picker.SelectedIndex = 0; // corresponds to value 2
			PlayerCount = 2;
		}

		// Do not wire button Clicked handlers here because they are already specified in XAML.

		// ensure card UI reflects initial state
		UpdateCardUi();
	}

	private void PlayerCountPicker_SelectedIndexChanged(object sender, EventArgs e)
	{
		if (sender is Picker picker && picker.SelectedIndex >= 0)
		{
			PlayerCount = int.Parse(picker.Items[picker.SelectedIndex]);
		}
	}

	// Start the game: hide the initial controls and show the card container with first card
	private async void StartGameButton_Clicked(object? sender, EventArgs e)
	{
		// hide initial UI controls
		var playerLabel = this.FindByName<Label>("PlayerCountLabel");
		if (playerLabel != null) playerLabel.IsVisible = false;

		var tipLabel = this.FindByName<Label>("PlayerCountTip");
		if (tipLabel != null) tipLabel.IsVisible = false;

		var picker = this.FindByName<Picker>("PlayerCountPicker");
		if (picker != null) picker.IsVisible = false;

		var startBtn = this.FindByName<Button>("StartGameButton");
		if (startBtn != null) startBtn.IsVisible = false;

		var cardContainer = this.FindByName<Grid>("CardContainer");
		if (cardContainer != null) cardContainer.IsVisible = true;

		// show loading indicator while cards load
		var loader = this.FindByName<ActivityIndicator>("CardsLoadingIndicator");
		if (loader != null)
		{
			loader.IsVisible = true;
			loader.IsRunning = true;
		}

		try
		{
			await EnsureCardsLoadedAsync();

			// randomize cards before starting (shuffle after load)
			ShuffleCards();

			// start at first card
			_currentCardIndex = 0;
			UpdateCardUi();
		}
		finally
		{
			if (loader != null)
			{
				loader.IsRunning = false;
				loader.IsVisible = false;
			}
		}
	}

	private void PrevCardButton_Clicked(object? sender, EventArgs e)
	{
		if (_currentCardIndex > 0)
		{
			_currentCardIndex--;
			UpdateCardUi();
		}
	}

	private void NextCardButton_Clicked(object? sender, EventArgs e)
	{
		if (_cards == null) return;
		if (_currentCardIndex < _cards.Count - 1)
		{
			_currentCardIndex++;
			UpdateCardUi();
		}
	}

	private void UpdateCardUi()
	{
		var cardLabel = this.FindByName<Label>("CardTextLabel");
		var prevBtn = this.FindByName<Button>("PrevCardButton");
		var nextBtn = this.FindByName<Button>("NextCardButton");
		var playerTurnLabel = this.FindByName<Label>("PlayerTurnLabel");

		// ensure at least 1 player to avoid division by zero or negative
		int players = Math.Max(1, PlayerCount);

		if (_cards != null && _currentCardIndex >= 0 && _currentCardIndex < _cards.Count)
		{
			// compute current player number (1-based). Cycle through players as cards progress.
			int playerNumber = (_currentCardIndex % players) + 1;
			if (playerTurnLabel != null) playerTurnLabel.Text = $"Player {playerNumber}'s turn";

			if (cardLabel != null) cardLabel.Text = _cards[_currentCardIndex];
			if (prevBtn != null) prevBtn.IsEnabled = _currentCardIndex > 0;
			if (nextBtn != null) nextBtn.IsEnabled = _currentCardIndex < _cards.Count - 1;
		}
		else
		{
			if (playerTurnLabel != null) playerTurnLabel.Text = string.Empty;
			if (cardLabel != null) cardLabel.Text = string.Empty;
			if (prevBtn != null) prevBtn.IsEnabled = false;
			if (nextBtn != null) nextBtn.IsEnabled = false;
		}
	}

	// Fisher–Yates shuffle to randomize the _cards list in-place
	private void ShuffleCards()
	{
		if (_cards == null || _cards.Count <= 1) return;

		for (int i = _cards.Count - 1; i > 0; i--)
		{
			int j = _rng.Next(i + 1);
			// swap
			var tmp = _cards[i];
			_cards[i] = _cards[j];
			_cards[j] = tmp;
		}
	}

	private async Task EnsureCardsLoadedAsync()
	{
		if (_cards != null) return;

		// Load cards.json from app package (Resources/Raw)
		try
		{
			using var stream = await FileSystem.OpenAppPackageFileAsync("cards.json");
			using var sr = new StreamReader(stream);
			var json = await sr.ReadToEndAsync();
			_cards = JsonSerializer.Deserialize<List<string>>(json) ?? new List<string>();
		}
		catch (Exception ex)
		{
			// fallback to empty list on error
			_cards = new List<string>();
			Console.WriteLine($"Failed to load cards.json: {ex}");
		}
	}

}