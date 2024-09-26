namespace HissEllerDissApi.Models.HissEllerDiss
{
    public static class HissEllerDissExtensions
    {
        public static HissEllerDissEntry ToHissEllerDissEntry(this IHissEllerDissEntry entry)
        {
            return new HissEllerDissEntry(entry.Name, entry.Likes);
        }

        public static HissEllerDissEntry IncreaseVote(this IHissEllerDissEntry entry, long votes)
        {
            if (votes < 0)
                throw new ArgumentException("Votes must be positive");
            
            return new HissEllerDissEntry(entry.Name, entry.Likes + votes);
        }

        public static HissEllerDissEntry DecreaseVote(this IHissEllerDissEntry entry, long votes)
        {
            if (votes > 0)
                throw new ArgumentException("Votes must be negative");
            
            return new HissEllerDissEntry(entry.Name, entry.Likes - votes);
        }

        public static HissEllerDissEntry ResetVote(this IHissEllerDissEntry entry)
        {
            return new HissEllerDissEntry(entry.Name, 0);
        }
    }
}
