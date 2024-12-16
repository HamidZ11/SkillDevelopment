using System; 
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Runtime.CompilerServices; // Allows using List<T>
using System.Threading;

class Program // The "Program" class is the main container for the code
{
    
    static void Main(string[] args)
    {
        Console.WriteLine();//blank line
        Console.WriteLine("Welcome to blackjack, dealer stands on 17");

        Console.WriteLine();//blank line

        int userDeckRequirement = GetUserDeckRequirement(); // Call method to get the number of decks
        Console.WriteLine($"You selected {userDeckRequirement} decks."); 

        List<string> fullDeck = GenerateMultipleDecks(userDeckRequirement);

        ShuffleDeck(fullDeck); //fullDeck comes from GenerateMultipleDecks
        Console.WriteLine("The deck has been shuffled.");


        while (true)
        {

            if (fullDeck.Count < 10)
            {
                Console.WriteLine("Deck is running low on cards, Reshuffling...");
                fullDeck = GenerateMultipleDecks(userDeckRequirement);
                ShuffleDeck(fullDeck);
                Console.WriteLine("The deck has been reshuffled.");
            }
            // userDeckRequirement passed as the int parameter for the method
            Console.WriteLine($"Your deck contains {fullDeck.Count} cards.");

            Console.WriteLine();//blank line

            int numberOfHands = GetNumberOfHands(); // Ask how many hands the player wants
            Console.WriteLine($"You chose to play {numberOfHands} hand(s).");
            List<string> handNames = new List<string>();
            for (int i = 0; i < numberOfHands; i++)
            {
                handNames.Add($"Hand {i + 1}");
            }

            Console.WriteLine();//blank line

            List<double> stakes = GetStakesForHands(numberOfHands);

            Console.WriteLine("\nYour stakes for each hand are as follows:");

            for (int i = 0; i < stakes.Count; i++)
            {
                Console.WriteLine($"Hand {i + 1}: £{stakes[i]}");
            }

            Console.WriteLine();//blank line

            List<string> dealerHand = DealCards(fullDeck); //Only one hand for dealer 

            // Step 1: Deal all hands and display them
            List<List<string>> playerHands = new List<List<string>>();
            for (int i = 0; i < numberOfHands; i++)
            {
                List<string> playerHand = DealCards(fullDeck);
                playerHands.Add(playerHand); 
                int handValue = CalculateHandValue(playerHand);
                Thread.Sleep(1000);//1 second gap
                Console.WriteLine($"Player's hand {i + 1}: {string.Join(", ", playerHand)}");

                if (handValue == 21 && playerHand.Count == 2)
                {
                    Console.WriteLine("BlackJack !!!");
                }
            }

            // Show only the first card of the dealer's hand initially
            Console.WriteLine();//blank line
            Thread.Sleep(1000); //1 second gap
            Console.WriteLine($"Dealer's hand: {dealerHand[0]}, [Hidden Card]");

            // Step 2: Play each hand sequentially
            for (int i = 0; i < playerHands.Count; i++)
            {
                Console.WriteLine();//blank line
                Console.WriteLine($"Starting {handNames[i]} with stake £{stakes[i]}");
                Console.WriteLine($"Your current hand:\n\t{string.Join(", ", playerHands[i])}\n\t(Total Value: {CalculateHandValue(playerHands[i])})");
                
                List<string> playerHand = playerHands[i];
                bool isHandActive = true;
                while (isHandActive)
                {

                    int handValue = CalculateHandValue(playerHand);
                    // Check if the player has a Blackjack (natural 21 with 2 cards)

                    if (handValue == 21 && playerHand.Count == 2) //Check if player has BlackJack
                    {
                        Console.WriteLine($"Blackjack! 🎉 Your hand is complete: {string.Join(", ", playerHand)} (Total Value: {handValue})");
                        isHandActive = false; // End the turn for this hand
                        break;
                    }

                    if (handValue == 21)
                    {
                        Console.WriteLine($"21! Your hand is complete: {string.Join(", ", playerHand)} ");
                        isHandActive = false;
                        break;
                    }
                    // Action options and game logic (hit, stand, double down, split, etc.)
                    if (playerHand.Count == 2 && CalculateCardValue(playerHand[0]) == CalculateCardValue(playerHand[1]))
                    {
                        Console.WriteLine($"Options for {handNames[i]} (H)Hit, (S)Stand, (D)Double Down, (P)Split");
                    }
                    else
                    {
                        Console.WriteLine($"Options for {handNames[i]} (H)Hit, (S)Stand, (D)Double Down");
                    }

                    string action = Console.ReadLine()?.ToUpper() ?? "";

                    switch (action)
                    {
                        case "H": // Hit
                            Console.WriteLine("You chose to Hit.");
                            playerHand.Add(fullDeck[0]);
                            fullDeck.RemoveAt(0);
                            Console.WriteLine($"Your new hand: {string.Join(", ", playerHand)} (Total Value: {CalculateHandValue(playerHand)})");

                            if (CalculateHandValue(playerHand) > 21)
                            {
                                Console.WriteLine($"Bust! Your hand value is {CalculateHandValue(playerHand)}.");
                                isHandActive = false; // End the hand
                            }
                            break;

                        case "S": // Stand
                            Console.WriteLine("You chose to Stand.");
                            Console.WriteLine($"Final hand: {string.Join(", ", playerHand)} (Total Value: {CalculateHandValue(playerHand)})");
                            isHandActive = false;
                            break;

                        case "D": // Double Down
                            stakes[i] *= 2;
                            Console.WriteLine($"You doubled down! Your new stake is £{stakes[i]}.");
                            playerHand.Add(fullDeck[0]);
                            fullDeck.RemoveAt(0);
                            Console.WriteLine($"Your new hand: {string.Join(", ", playerHand)} (Total Value: {CalculateHandValue(playerHand)})");

                            if (CalculateHandValue(playerHand) > 21)
                            {
                                Console.WriteLine($"Bust! Your hand value is {CalculateHandValue(playerHand)}.");
                            }
                            isHandActive = false;
                            break;

                       case "P": // Split
                            if (playerHand.Count == 2 && CalculateCardValue(playerHand[0]) == CalculateCardValue(playerHand[1]))
                            {
                                Console.WriteLine($"You chose to Split {handNames[i]}.");

                                // Create two new hands from the split
                                List<string> splitHand1 = new List<string> { playerHand[0], fullDeck[0] };
                                fullDeck.RemoveAt(0);

                                List<string> splitHand2 = new List<string> { playerHand[1], fullDeck[0] };
                                fullDeck.RemoveAt(0);

                                playerHands[i] = splitHand1;

                                playerHands.Insert(i + 1, splitHand2);

                                stakes.Insert(i + 1, stakes[i]);

                                string originalHandName = handNames[i];
                                handNames[i] = $"{originalHandName} > Split 1";
                                handNames.Insert(i + 1, $"{originalHandName} > Split 2");

                                Console.WriteLine($"{handNames[i]}: {string.Join(", ", splitHand1)} (Total Value: {CalculateHandValue(splitHand1)})");
                                Console.WriteLine($"{handNames[i + 1]}: {string.Join(", ", splitHand2)} (Total Value: {CalculateHandValue(splitHand2)})");

                                Console.WriteLine($"Playing {handNames[i]}...");
                                ProcessHand(playerHands[i], fullDeck, playerHands, stakes, i);

                                 i++;
                            }
                            else
                            {
                                Console.WriteLine("You cannot Split this hand.");
                            }
                            break;


                        default:
                            Console.WriteLine("Invalid action. Please choose H, S, D, or P.");
                            break;
        }
    }
}


            // At the end of the round, show the dealer's full hand
            Console.WriteLine("\nRevealing dealer's full hand...");
            Thread.Sleep(1000); 
            Console.WriteLine($"Dealer's hand: {string.Join(", ", dealerHand)} (Total Value: {CalculateHandValue(dealerHand)})");
            //Dealer Actions
            while (CalculateHandValue(dealerHand) < 17)
            {
                Thread.Sleep(1000);
                dealerHand.Add(fullDeck[0]);
                fullDeck.RemoveAt(0);
                Console.WriteLine($"Dealer hits and gets: {dealerHand[^1]} (Total Value: {CalculateHandValue(dealerHand)})");
            }
            if (CalculateHandValue(dealerHand) > 21)
            {
                Console.WriteLine("Dealer Busts!");
            }
            else 
            {
                Console.WriteLine($"Dealer stands with hand: {string.Join(", ", dealerHand)} (Total Value: {CalculateHandValue(dealerHand)})");
            }

            DetermineWinners(playerHands, dealerHand, stakes);

            //Play Again
            Console.WriteLine("Do you want to play another round? (Y/N)");
            string playAgain = (Console.ReadLine() ?? "").ToUpper();

            if (playAgain != "Y")
            {
                Console.WriteLine("Thanks for playing, Goodbye !");
                break;
            }

        }       
    }

    
    static List<string> GenerateDeck()
    {
        string[] suits = { "Hearts", "Diamonds", "Clubs", "Spades" }; 
        string[] ranks = { "2", "3", "4", "5", "6", "7", "8", "9", "10", "Jack", "Queen", "King", "Ace" }; // Define the ranks
        List<string> deck = new List<string>(); // Create an empty list for the deck

        // Outer Loop: Goes through each suit
        foreach (var suit in suits)
        {
            // Inner Loop: Goes through each rank
            foreach (var rank in ranks)
            {
                deck.Add($"{rank} of {suit}"); // Add each card to the deck 
            }
        }

        return deck; // "return" sends this list back to the code that called the method
    }

    static List<string> GenerateMultipleDecks(int numberOfDecks) 
    {
        List<string> singleDeck = GenerateDeck();
        List<string> combinedDeck = new List<string>();

        for (int i=0; i < numberOfDecks; i++)
        {
            combinedDeck.AddRange(singleDeck);
        }

        return combinedDeck;
    }


    static int GetUserDeckRequirement()
    {
        while (true) 
        {
            try // "try" is where code is attempted to run; if something goes wrong, the program jumps to "catch"
            {
                Console.WriteLine("How many decks would you like to play with? (1-6)"); 
                int userDeckRequirement = int.Parse(Console.ReadLine() ?? ""); 

                if (userDeckRequirement >= 1 && userDeckRequirement <= 6) 
                {
                    return userDeckRequirement; 
                }
                else
                {
                    Console.WriteLine("Invalid number of decks. Please enter a number between 1 and 6."); 
                }
            }
            catch (FormatException) // Executes if a parsing error happens in the "try" block
            {
                Console.WriteLine("Invalid input. Please enter a number between 1-6."); 
            }
        }
    }

    static void ShuffleDeck(List<string> deck)
    {
        Random rng = new Random();

        for (int i = deck.Count - 1; i>0; i--) //start at last card and move down
        {
            int swapIndex = rng.Next(i +1);
            string temp = deck[i];
            deck[i] = deck[swapIndex];
            deck[swapIndex] = temp; 
        }
    }
    
    static int GetNumberOfHands() //similar to GetNumberofDecks
    {
        while (true)
        {
            try
            {
               Console.WriteLine("How many hands would you like to play? (1-5)");
                int numberOfHands = int.Parse(Console.ReadLine() ?? "");

                if (numberOfHands >= 1 && numberOfHands <= 5)
                {
                    return numberOfHands;
                }
                else
                {
                    Console.WriteLine("Invalid number of hands. Please enter a number between 1 and 5.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid input. Please enter a valid number between 1-5.");
            }
        }
    }

    static List<double> GetStakesForHands(int numberOfHands)
    {
        List<double> stakes = new List<double>();
        
        for (int i = 1; i <= numberOfHands; i++)
        {
            double stake = GetStakeForHand(i);
            stakes.Add(stake);
        }

        return stakes;
    }

    static double GetStakeForHand(int handNumber)
    {
        while (true)
        {
            try
            {
                Console.WriteLine($"Enter the stake for Hand {handNumber} (minimum £5, whole numbers only):");
                double stake = double.Parse(Console.ReadLine() ?? "");

                if (stake >= 5 && stake % 1 == 0) 
                {
                    return stake;
                }
                else
                {
                    Console.WriteLine("Invalid stake. Please enter a whole number stake of at least £5.");
                }
            }
            catch (FormatException)
            {
                Console.WriteLine("Invalid stake. Please enter a stake above £5, whole bets only");
            }
        }
    }

    static List<string> DealCards(List<string> deck)
    {
        List<string> hand = new List<string>();
        hand.Add(deck[0]); // Deal the first card
        deck.RemoveAt(0); // Remove the card from the deck

        hand.Add(deck[0]); // Deal the second card
        deck.RemoveAt(0); // Remove the card from the deck

        return hand;
    }

    static int CalculateHandValue(List<string> hand)
    {
        int totalValue = 0;
        int aceCount = 0;

        foreach (string card in hand)
        {
            string rank = card.Split(' ')[0]; //Extract the rank, so like 2 from 2 of hearts, 2 is in [0] position

            if (int.TryParse(rank, out int numbericValue))
            {
                totalValue += numbericValue;
            }
            else if (rank == "Jack" || rank == "Queen" || rank == "King")
            {
                totalValue += 10;
            }
            else if (rank =="Ace")
            {
                aceCount++;
                totalValue += 11;
            }

        }
        while (totalValue > 21 && aceCount > 0)
        {
            totalValue -= 10;
            aceCount --;
        }
        return totalValue;
    }

    static int CalculateCardValue(string card)
    {
        string rank = card.Split(' ')[0];

        if(int.TryParse(rank, out int numbericValue))
        {
            return numbericValue;
        }
        else if (rank == "Jack" || rank == "Queen" || rank =="King")
        {
            return 10;
        }
        else if (rank == "Ace")
        {
            return 11; //Aces are handled separately in the total hand value calculation method
        }

        return 0; //fallback which should not occur

    }

    static void DetermineWinners(List<List<string>> playerHands, List <string> dealerHand, List<double> stakes)
    {
        int dealerValue = CalculateHandValue(dealerHand);
        bool dealerBlackjack = dealerValue == 21 && dealerHand.Count == 2;
        Console.WriteLine($"\nDealer's final hand: {string.Join(", ", dealerHand)} (Total value: {dealerValue})\n");

        for (int i = 0; i < playerHands.Count; i++)
        {
            int playerValue = CalculateHandValue(playerHands[i]);
            bool isBlackjack = playerValue == 21 && playerHands[i].Count == 2;

            if (playerValue > 21)
            {
                Console.WriteLine($"Hand {i + 1} busts! You lose £{stakes[i]}.");
            }
            else if (isBlackjack && dealerBlackjack)
            {
                // Both player and dealer have Blackjacks -> Push
                Console.WriteLine($"Hand {i + 1} ties with the dealer's Blackjack! Your stake of £{stakes[i]:0.00} is returned.");
            }
            else if (isBlackjack) 
            {
                double payout = stakes[i] * 2.5; // 2.
                Console.WriteLine($"Hand {i + 1} wins with Blackjack! You win £{payout:0.00} (Stake £{stakes[i]:0.00} + Winnings £{stakes[i] * 1.5:0.00}).");
            }
            else if (dealerValue > 21 || playerValue > dealerValue)
            {
                double payout = stakes[i] * 2;
                Console.WriteLine($"Hand {i + 1} wins! You win £{payout:0.00} (Stake £{stakes[i]:0.00} + Winnings £{stakes[i]:0.00}).");
            }
            else if (playerValue == dealerValue)
            {
                Console.WriteLine($"Hand {i + 1} pushes! Your stake of £{stakes[i]} is returned.");
            }
            else
            {
                Console.WriteLine($"Hand {i + 1} loses! You lose £{stakes[i]}.");
            }
        }
        
        Console.WriteLine(); // Blank line between hand results
    }

    static void ProcessHand(List<string> hand, List<string> deck, List<List<string>> playerHands, List<double> stakes, int handIndex)
    {
        bool isHandActive = true; 

        while (isHandActive)
        {
            Console.WriteLine($"Your current hand: {string.Join(", ", hand)} (Total Value: {CalculateHandValue(hand)})");
            Console.WriteLine($"Options: (H) Hit, (S) Stand, (D) Double Down");

            string action = Console.ReadLine()?.ToUpper() ?? "";

            switch (action)
            {
                case "H":
                    Console.WriteLine("You chose to Hit.");
                    hand.Add(deck[0]);
                    deck.RemoveAt(0);
                    Console.WriteLine($"Your new hand: {string.Join(", ", hand)} (Total Value: {CalculateHandValue(hand)})");
                    if (CalculateHandValue(hand) > 21)
                    {
                        Console.WriteLine($"Bust! Your hand value is {CalculateHandValue(hand)}.");
                        isHandActive = false;
                    }
                    break;

                case "S": // Stand
                    Console.WriteLine("You chose to Stand.");
                    Console.WriteLine($"Final hand: {string.Join(", ", hand)} (Total Value: {CalculateHandValue(hand)})");
                    isHandActive = false;
                    break;

                case "D":
                    stakes[handIndex] *= 2;
                    Console.WriteLine($"You doubled down! Your new stake is £{stakes[handIndex]}.");
                    hand.Add(deck[0]);
                    deck.RemoveAt(0);
                    Console.WriteLine($"Your new hand: {string.Join(", ", hand)} (Total Value: {CalculateHandValue(hand)})");

                    if (CalculateHandValue(hand) > 21)
                    {
                        Console.WriteLine($"Bust! Your hand value is {CalculateHandValue(hand)}.");
                    }
                    isHandActive = false;
                    break;

                    default:
                        Console.WriteLine("Invalid action. Please choose H, S, or D.");
                        break;

            }
        }
    }



}
