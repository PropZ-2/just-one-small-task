using System;
using System.Collections.Generic;

namespace just_one_small_task;

public partial class GamePage : ContentPage
{
	public int PlayerCount;

	// Simple in-memory list of cards. Each card is just a string for now.
	private List<string> _cards = new List<string>
	{
		"Card 1: You are now the DJ, play something awful in a non pop gerner for the next 10-15 minuits",
		"Card 2: Spil a bunch of water on your shirt",
		"Card 3: Give the previous player a simple fetch quest limited to 1 item",
		"Card 4: Swap shirts or pants or jacket with next player",
		"Card 5: Swap seats and player number and name with someone",
		"Card 6: You must only speak Thai from now on until someone says 'i will here by free you of your struggle' and kisses you on the cheeck",
		"Card 7: Act like an animal of the player infront of you's choise",
		"Card 8: If any make up is avaible another player of your choise will apply it to you, if thier is non avaible you have to go wash your face every 5 minuite",
		"Card 9: Imitate a fictional character of the previouse player's choise that you both know of",
		"Card 10: lick a wall or table or a chair",
		"Card 11: eat a bit of toilet papir",
		"Card 12: try to convince someone to suck on your finger within 1 min if you fail you have to sit on the floor",
		"Card 13: in 10 secounds the floor is will be lava for all players first one to touch the floor will have to moan after every sentence",
        "Card 14: next player get call one of your friend, if they pick up they have to keep talking for 1 min or you both have to swap names who ever reacts to thier old name first have to change the profile picture on a social media to 'penis cat meme' for a week (look it up it's the cat with penis shape on it's face)",
		"Card 15: change your wall paper to a picture of someone else for the rest of the week",
		"Card 16: "
    };

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
	private void StartGameButton_Clicked(object? sender, EventArgs e)
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

		// randomize cards before starting
		ShuffleCards();

		// start at first card
		_currentCardIndex = 0;
		UpdateCardUi();
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

		if (_currentCardIndex >= 0 && _currentCardIndex < _cards.Count)
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
		for (int i = _cards.Count - 1; i > 0; i--)
		{
			int j = _rng.Next(i + 1);
			// swap
			var tmp = _cards[i];
			_cards[i] = _cards[j];
			_cards[j] = tmp;
		}
	}

}