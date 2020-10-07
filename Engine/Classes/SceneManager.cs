namespace Aximo.Engine
{
    public static class SceneManager
    {
        private static Scene CurrentScene;

        public static void SetActiveScene(Scene scene)
        {
            CurrentScene = scene;
            scene.Enabled = true;
            foreach (var act in scene.GetActors())
                act.OnSceneActivated();
        }

        public static Scene GetCurrentScene()
        {
            return CurrentScene;
        }

    }

}
