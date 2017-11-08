namespace Chess.Featuriser.Cli
{
    public interface IConfigurationOptions
    {
        bool Help { get; set; }
        string ConfigurationFile { get; set; }

        bool Validate();
    }
}
