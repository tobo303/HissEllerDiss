namespace HissEllerDissApi.Models.HissEllerDiss;

public interface IHissEllerDissEntry
{
    int Id { get; }
    string Name { get; }
    long Likes { get; }
}