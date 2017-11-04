namespace Chess.Featuriser
{
    public interface IConfigurationOptions
    {
        bool Help { get; set; }
        string ConfigurationFile { get; set; }

        bool Validate();
    }
}
