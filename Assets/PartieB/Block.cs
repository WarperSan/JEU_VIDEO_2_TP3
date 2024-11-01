namespace PartieB
{
    public enum Block
    {
        AIR = 23 + 24 * 0,

        // GROUND
        BEDROCK = 1 + 24 * 32,
        GRASS = 0 + 24 * 33,
        DIRT = 2 + 24 * 33, 
        STONE = 1 + 24 * 33,

        // ORES
        ORE_IRON = 1 + 24 * 31,
        ORE_DIAMOND = 2 + 24 * 30,

        // SPECIALS
        GLASS = 1 + 24 * 30,
        SPAWNER = 1 + 24 * 29
    }
}