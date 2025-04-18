﻿using System;
using System.IO;
using System.Text.Json;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace SpaceWar;

public class Game1 : Game
{
    private GraphicsDeviceManager _graphics;
    private SpriteBatch _spriteBatch;
    private Screen currentScreen;

    public SpriteFont TextFont { get; private set; }
    public SpriteFont TitleFont { get; private set; }
    public SpriteFont MidFont { get; private set; }

    public GameOptions GameOptions { get; set; }
    public Point ScreenResolution = new Point(1280, 720);
    public readonly GameOptions DefaultOptions = new() {
        Speed = 15f,
        BoostedSpeed = 30f,
        MaxBullets = 5,
        BulletSpeed = 10f,
        InfiniteBoost = false
    };

    private static readonly string OptionsFilePath = "options.json";

    public Game1()
    {
        _graphics = new GraphicsDeviceManager(this);
        Content.RootDirectory = "Content";
        _graphics.PreferredBackBufferWidth = ScreenResolution.X;
        _graphics.PreferredBackBufferHeight = ScreenResolution.Y;
        IsMouseVisible = true;
        _graphics.IsFullScreen = false;
    }

    protected override void Initialize()
    {
        currentScreen = new MenuScreen(this);
        base.Initialize();
        GameOptions = loadOptions();
    }

    protected override void LoadContent()
    {
        _spriteBatch = new SpriteBatch(GraphicsDevice);
        TitleFont = Content.Load<SpriteFont>("TitleFont");
        MidFont = Content.Load<SpriteFont>("MidFont");
        TextFont = Content.Load<SpriteFont>("TextFont");

        currentScreen.LoadContent(Content);
    }

    protected override void Update(GameTime gameTime)
    {
        currentScreen.Update(gameTime);
        base.Update(gameTime);
    }

    protected override void Draw(GameTime gameTime)
    {
        GraphicsDevice.Clear(Color.Black);
        _spriteBatch.Begin();
        currentScreen.Draw(_spriteBatch);
        _spriteBatch.End();
        base.Draw(gameTime);
    }

    public void ChangeScreen(Screen newScreen) {
        newScreen.LoadContent(Content);
        currentScreen = newScreen;
    }

    private GameOptions loadOptions() {
        try {
            if (File.Exists(OptionsFilePath)) {
                string json = File.ReadAllText(OptionsFilePath);
                return JsonSerializer.Deserialize<GameOptions>(json) ?? DefaultOptions;
            }
        } catch (Exception e) {
            Console.WriteLine($"Erreur chargement options : {e.Message}");
        }
        return DefaultOptions;
    }

    public void SaveOptions(GameOptions options) {
        try {
            string json = JsonSerializer.Serialize(options, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(OptionsFilePath, json);
        } catch (Exception e) {
            Console.WriteLine($"Erreur sauvegarde options : {e.Message}");
        }
    }

    public bool AreDefaultOptions() {
        GameOptions currentOptions = GameOptions;
        GameOptions defaultOptions = DefaultOptions;
        return currentOptions.Speed == defaultOptions.Speed &&
            currentOptions.BoostedSpeed == defaultOptions.BoostedSpeed &&
            currentOptions.MaxBullets == defaultOptions.MaxBullets &&
            currentOptions.BulletSpeed == defaultOptions.BulletSpeed &&
            currentOptions.InfiniteBoost == defaultOptions.InfiniteBoost;
    }
}
