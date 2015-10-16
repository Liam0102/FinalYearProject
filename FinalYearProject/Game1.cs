using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using System;
using System.Diagnostics;

namespace FinalYearProject
{
    /// <summary>
    /// This is the main type for your game.
    /// </summary>
    public class Game1 : Game
    {
        GraphicsDeviceManager graphics;
        SpriteBatch spriteBatch;

        Texture2D board;
        Texture2D cardholder;
        Card[] cards = new Card[5];
        CardHolder[] holders = new CardHolder[5];
        SpriteFont arial16;
        public Random rnd = new Random();
        int CardSeperation;

        int CardWidth, CardHeight;
        public int Goal;
        public Game1()
        {
            graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
        }

        /// <summary>
        /// Allows the game to perform any initialization it needs to before starting to run.
        /// This is where it can query for any required services and load any non-graphic
        /// related content.  Calling base.Initialize will enumerate through any components
        /// and initialize them as well.
        /// </summary>
        private string[] Comb = new string[]
        {
            "01234",
            "10234",
            "12034",
            "12304",
            "12340",
            "02134",
            "02314",
            "02341",
            "20134",
            "02134",
            "01324",
            "01342",
            "30124",
            "03124",
            "01324",
            "01243",
            "40123",
            "04123",
            "01423",
            "01243",
        };

        public CardHolder[] opHolders = new CardHolder[4];
        public Card[] opLeft = new Card[4];
        protected override void Initialize()
        {
            // TODO: Add your initialization logic here
            graphics.PreferredBackBufferWidth = 1280;
            graphics.PreferredBackBufferHeight = 720;
            graphics.ApplyChanges();

            CardWidth = Convert.ToInt32(graphics.PreferredBackBufferWidth / 6);
            CardHeight = Convert.ToInt32(graphics.PreferredBackBufferHeight / 2);

            CardSeperation = (graphics.PreferredBackBufferWidth - (CardWidth * 5)) / 6;

            this.IsMouseVisible = true;
            int Seed = (int)DateTime.Now.Ticks;
            Random rnd = new Random(Seed);
            for (int i = 0; i < cards.Length; i++)
            {
                
                int type = rnd.Next(1,4);
                
                cards[i] = new Card(type);
                cards[i].DeckPosX = Convert.ToInt32(graphics.PreferredBackBufferWidth/2 - (CardWidth/2*2.5) + (CardWidth/2* i) + (CardSeperation/2*i) - CardWidth/4);
                cards[i].xPos = cards[i].DeckPosX;
                cards[i].DeckPosY = graphics.PreferredBackBufferHeight - CardHeight / 10 * 4;
                cards[i].yPos = cards[i].DeckPosY;
                cards[i].Width = CardWidth / 2;
                cards[i].Height = CardHeight / 2;
                cards[i].InDeck = true;

                holders[i] = new CardHolder(((i + 1) * CardSeperation) + (i * CardWidth), CardHeight / 100 * 2, CardWidth, CardHeight);

            }

            int w = CardWidth / 3;
            int h = CardHeight / 3;
            for (int i = 0; i < opHolders.Length; i++)
            {
                opHolders[i] = new CardHolder(holders[i].xPos + holders[i].Width - ((w - CardSeperation) / 2), Convert.ToInt32(holders[i].yPos + holders[i].Height * 1.2), w, h);
            }

            string comb = Comb[rnd.Next(0, Comb.Length + 1)];
            Debug.WriteLine(comb);
            int[] index = new int[5];
            for(int i = 0; i < comb.Length; i++)
            {
                index[i] = Convert.ToInt32(comb.Substring(i, 1));
            }

            Goal = cards[index[0]].GetRating() + cards[index[1]].GetRating() - cards[index[2]].GetRating() * cards[index[3]].GetRating() / cards[index[4]].GetRating();

            Debug.WriteLine(Goal);

            base.Initialize();
        }

        private MouseState oldState;
        public void MouseInput()
        {
            MouseState newState = Mouse.GetState();
            int x = newState.X;
            int oldX = oldState.X;
            
            int y = newState.Y;
            int oldY = oldState.Y;

            if (newState.LeftButton == ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
            {
                for (int i = 0; i < cards.Length; i++)
                {

                    if ((cards[i].xPos < x && cards[i].xPos + cards[i].Width > x) && (cards[i].yPos < y && cards[i].yPos + cards[i].Height > y))
                    {
                        if (GetHoldingCard() == null || GetHoldingCard() == cards[i])
                        {
                            cards[i].xPos += x - oldX;
                            cards[i].yPos += y - oldY;
                            cards[i].Selected = true;
                        }
                    }
                    


                }
            }
            else if(newState.LeftButton != ButtonState.Pressed && oldState.LeftButton == ButtonState.Pressed)
            {
                Card cCard = GetHoldingCard();
                if (cCard != null)
                {
                    if (y < holders[0].yPos + holders[0].Height * 1.1)
                    {

                        for (int j = 0; j < holders.Length; j++)
                        {
                            if (cCard.xPos > holders[j].xPos && cCard.xPos < holders[j].xPos + holders[j].Width)
                            {
                                if (holders[j].currentCard == null)
                                {
                                    cCard.Holder = j;
                                    cCard.InDeck = false;
                                    cCard.xPos = holders[j].xPos;
                                    cCard.yPos = holders[j].yPos;
                                    holders[j].currentCard = cCard;
                                }
                            }
                        }
                    }
                    else
                    {
                        if (!cCard.InDeck)
                        {
                            cCard.InDeck = true;
                            holders[cCard.Holder].currentCard = null;
                            cCard.Holder = -1;
                            cCard.xPos = cCard.DeckPosX;
                            cCard.yPos = cCard.DeckPosY;
                        }
                    }
                }
                
            }
            else
            {
                for (int i = 0; i < cards.Length; i++)
                {
                    cards[i].Selected = false;
                    if (cards[i].InDeck)
                    {
                        if ((cards[i].xPos < x && cards[i].xPos + cards[i].Width > x) && (cards[i].yPos < y && cards[i].yPos + cards[i].Height > y))
                        {
                            cards[i].Width = CardWidth;
                            cards[i].Height = CardHeight;

                            cards[i].xPos = cards[i].DeckPosX - CardWidth / 4;
                            cards[i].yPos = Convert.ToInt32(cards[i].DeckPosY - CardHeight / 1.5);
                        }
                        else
                        {
                            cards[i].Width = CardWidth / 2;
                            cards[i].Height = CardHeight / 2;

                            cards[i].xPos = cards[i].DeckPosX;
                            cards[i].yPos = cards[i].DeckPosY;
                        }
                    }
                }
            }

            oldState = newState; // this reassigns the old state so that it is ready for next time
        }

        public int CalculateSolution()
        {
            int sol = 0;


            Debug.WriteLine(8 * 7 + 6 - 1 / 3);
            
            return sol;
        }


        public Card GetHoldingCard()
        {
            for(int i = 0; i < cards.Length; i++)
            {
                if (cards[i].Selected)
                {
                    return cards[i];
                }
            }
            return null;
        }

        /// <summary>
        /// LoadContent will be called once per game and is the place to load
        /// all of your content.
        /// </summary>
        protected override void LoadContent()
        {
            // Create a new SpriteBatch, which can be used to draw textures.
            spriteBatch = new SpriteBatch(GraphicsDevice);
            board = Content.Load<Texture2D>("board");
            cardholder = Content.Load<Texture2D>("cards/card-holder");

            for(int i = 0; i < cards.Length; i++)
            {
                cards[i].cTexture = Content.Load<Texture2D>(cards[i].TexturePath);
            }
            arial16 = Content.Load<SpriteFont>("fonts/arial-16");
            // TODO: use this.Content to load your game content here
        }

        /// <summary>
        /// UnloadContent will be called once per game and is the place to unload
        /// game-specific content.
        /// </summary>
        protected override void UnloadContent()
        {
            // TODO: Unload any non ContentManager content here
        }

        /// <summary>
        /// Allows the game to run logic such as updating the world,
        /// checking for collisions, gathering input, and playing audio.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Update(GameTime gameTime)
        {
            if (GamePad.GetState(PlayerIndex.One).Buttons.Back == ButtonState.Pressed || Keyboard.GetState().IsKeyDown(Keys.Escape))
                Exit();

            // TODO: Add your update logic here
            MouseInput();

            for(int i = 0; i < cards.Length; i++)
            {
                if (!cards[i].InDeck && !cards[i].Selected)
                {
                    cards[i].xPos = holders[cards[i].Holder].xPos;
                    cards[i].yPos = holders[cards[i].Holder].yPos;
                }
            }

            base.Update(gameTime);
        }

        /// <summary>
        /// This is called when the game should draw itself.
        /// </summary>
        /// <param name="gameTime">Provides a snapshot of timing values.</param>
        protected override void Draw(GameTime gameTime)
        {
            GraphicsDevice.Clear(Color.LightGoldenrodYellow);
            spriteBatch.Begin();

            //Draw the Card Holders
            for(int i = 0; i < holders.Length; i++)
            {
                spriteBatch.Draw(cardholder, new Rectangle(holders[i].xPos, holders[i].yPos, holders[i].Width, holders[i].Height), Color.White);
                int CardRating = 0;
                if(holders[i].currentCard != null)
                {
                  
                    CardRating = holders[i].currentCard.GetRating();
                    
                }
                spriteBatch.DrawString(arial16, CardRating.ToString(), new Vector2(holders[i].xPos + holders[i].Width / 2, Convert.ToInt32(holders[i].yPos + holders[i].Height * 1.1)), Color.Black);
                spriteBatch.DrawString(arial16, Goal.ToString(), new Vector2(graphics.PreferredBackBufferWidth / 2, graphics.PreferredBackBufferHeight / 4 * 3), Color.Black);
            }

            for(int i = 0; i < opHolders.Length; i++)
            {
                spriteBatch.Draw(cardholder, new Rectangle(opHolders[i].xPos, opHolders[i].yPos, opHolders[i].Width, opHolders[i].Height), Color.White);
            }

            //Draw the Cards
            for (int i = 0; i < cards.Length; i++)
            {
                spriteBatch.Draw(cards[i].cTexture, new Rectangle(cards[i].xPos, cards[i].yPos, cards[i].Width, cards[i].Height), Color.White);
            }

            spriteBatch.End();
            // TODO: Add your drawing code here

            base.Draw(gameTime);
        }

        public struct CardHolder
        {
            public int xPos, yPos;
            public int Width, Height;
            public bool Holding;
            public Card currentCard;
            public CardHolder(int x, int y, int w, int h)
            {
                this.xPos = x;
                this.yPos = y;
                this.Width = w;
                this.Height = h;
                this.Holding = false;
                this.currentCard = null;
            }
        }

    }

    public class Card
    {
        private int Type;
        private int Rating;
        public int xPos, yPos;
        public int Width, Height;
        public string TexturePath;
        public Texture2D cTexture;
        public bool InDeck;
        public int DeckPosY, DeckPosX;
        public bool Selected;
        public int Holder;
        public Card(int type)
        {
            this.Type = type;
            int Seed = (int)DateTime.Now.Ticks;
            Random rnd = new Random(Seed);
            this.Rating = 0;
            if (type == 1)
            {
                this.Rating = rnd.Next(1, 4);
            }
            else if (type == 2)
            {
                this.Rating = rnd.Next(4, 8);
            }
            else if (type == 3)
            {
                this.Rating = rnd.Next(8, 11);
            }

            this.TexturePath = "cards/" + type + "/" + this.Rating;
        }

        public int GetRating()
        {
            return this.Rating;
        }

        public int GetType()
        {
            return this.Type;
        }
    }
}
