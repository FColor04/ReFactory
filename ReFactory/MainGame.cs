﻿using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using System;
using AudioManagementUtil;
using ObscurusDebuggerTools;
using ReFactory.Controllers;
using ReFactory.GameScripts;
using ReFactory.Utility;
using ReFactory;
using ReFactory.UISystem;
using ReFactory.UISystem.LayoutControllers;

namespace MainGameFramework {
    public class MainGame : Game
    {
        private const int RenderTargetWidth = 320;
        private const int RenderTargetHeight = 180;

        #region Exposed Actions

        //Method like initialized & load content were also exposed,
        // but realistically nothing game related should be instantiated at this point so the methods are redundant
        //If you have need for initialize action, then just call a method from initialize directly.

        /// <summary>
        /// Called within <see cref="Update"/>>
        /// </summary>
        public static event Action<float> OnUpdate = _ => { };
        /// <summary>
        /// Called within <see cref="Draw"/>, for usage of main sprite batch use <see cref="OnDrawSprites"/>>
        /// <remarks>This is called before <see cref="OnDrawSprites"/></remarks>
        /// </summary>
        public static event Action<float> OnDraw = _ => { };
        /// <summary>
        /// Called within <see cref="Draw"/>, provides pixel art sprite batch to draw sprites within.
        /// <remarks>This is called after <see cref="OnDraw"/></remarks>
        /// </summary>
        public static event Action<float, SpriteBatch> OnDrawSprites = (_, _) => { };
        /// <summary>
        /// Called within <see cref="Draw"/>, provides batch rendered after other batches to draw UI.
        /// <remarks>This is called after <see cref="OnDrawSprites"/></remarks>
        /// </summary>
        public static event Action<float, SpriteBatch> OnDrawUI = (_, _) => { };

        #endregion

        public static MainGame Instance;
        public static GraphicsDevice graphicsDevice => Instance.GraphicsDevice;
        private readonly GraphicsDeviceManager _graphics;
        public static GraphicsDeviceManager graphicsDeviceManager => Instance._graphics;
        public static ContentManager content => Instance.Content;
        public static Point WindowSize => new Point(Instance._graphics.PreferredBackBufferWidth, Instance._graphics.PreferredBackBufferHeight);
        public static bool IsFocused => Instance.IsActive;

        private SpriteBatch _spriteBatch;
        private RenderTarget2D _renderTarget;
        private StateMachine playerStateMachine;
        private int worldSizeX = 17;
        private int worldSizeY = 17;
        public World World;
        public int WorldSizeX { get { return worldSizeX; } }
        public int WorldSizeY { get { return worldSizeY; } }

        public MainGame()
        {
            Instance = this;
            _graphics = new GraphicsDeviceManager(this);
            Content.RootDirectory = "Content";
            IsMouseVisible = true;
            Window.AllowUserResizing = true;
            Debug.LogError("this is test error");
        }

        protected override void Initialize()
        {
            base.Initialize();
        }

        protected override void LoadContent()
        {
            _spriteBatch = new SpriteBatch(GraphicsDevice);
            _renderTarget = new RenderTarget2D(GraphicsDevice, RenderTargetWidth, RenderTargetHeight);

            GameContent.RegisterItems();

            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(AudioManager).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(UI).TypeHandle);
            System.Runtime.CompilerServices.RuntimeHelpers.RunClassConstructor(typeof(Resolution).TypeHandle);

            var toolbar = new UIElement(new UI.Margin(180 - 18, 88, 0, 88), UI.Pixel, ColorUtility.ToolbarGrey);
            UI.Root.AddChild(toolbar);
            for (int i = 0; i < 8; i++)
            {
                var index = i;
                var itemSlot = new ItemSlot(
                    () => Inventory.items[index],
                    item => Inventory.items[index] = item,
                    new Rectangle(0, 0, 16, 16),
                    UI.Pixel,
                    ColorUtility.ToolbarSilver);
                toolbar.AddChild(itemSlot);
            }

            Inventory.AddItem(new Item("Belt", 24));

            toolbar.ProcessUsingLayoutController(new HorizontalGrid(toolbar, new UI.Margin(1)));
            toolbar.ProcessUsingLayoutController(new FixedSize(16, 16));

            World = new World(WorldSizeX, WorldSizeY);
        }

        protected override void Update(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;

            Time.UnscaledDeltaTime = deltaTime;
            Time.UnscaledTotalTime = (float)gameTime.TotalGameTime.TotalSeconds;

            deltaTime *= Time.TimeScale;

            Time.TotalTime += deltaTime;
            Time.DeltaTime = deltaTime;

            //Input is exception so it's executed before other stuff not to cause race conditions.
            Input.UpdateState();
            if (Input.Exit)
                Exit();

            //Everything else should subscribe to OnUpdate event
            OnUpdate?.Invoke(deltaTime);

            base.Update(gameTime);
        }

        protected override void Draw(GameTime gameTime)
        {
            float deltaTime = (float)gameTime.ElapsedGameTime.TotalSeconds;
            GraphicsDevice.SetRenderTarget(_renderTarget);
            GraphicsDevice.Clear(Color.DarkGoldenrod);

            OnDraw?.Invoke(deltaTime);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);

            OnDrawSprites?.Invoke(deltaTime, _spriteBatch);
            OnDrawUI?.Invoke(deltaTime, _spriteBatch);

            _spriteBatch.End();

            GraphicsDevice.SetRenderTarget(null);
            GraphicsDevice.Clear(Color.Black);

            _spriteBatch.Begin(samplerState: SamplerState.PointClamp);
            _spriteBatch.Draw(_renderTarget, Resolution.TrimmedScreen, Color.White);
            _spriteBatch.End();

            base.Draw(gameTime);
        }
    }
}