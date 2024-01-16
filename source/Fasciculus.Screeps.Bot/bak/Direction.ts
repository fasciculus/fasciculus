
export class Direction
{
    readonly dx: number;
    readonly dy: number;

    private constructor(dx: number, dy: number)
    {
        this.dx = dx;
        this.dy = dy;
    }

    static readonly none = new Direction(0, 0);
    static readonly north = new Direction(0, -1);
    static readonly northEast = new Direction(1, -1);
    static readonly east = new Direction(1, 0);
    static readonly southEast = new Direction(1, 1);
    static readonly south = new Direction(0, 1);
    static readonly southWest = new Direction(-1, 1);
    static readonly west = new Direction(-1, 0);
    static readonly northWest = new Direction(-1, -1);

    static get neighbours(): Direction[]
    {
        return [this.north, this.northEast, this.east, this.southEast, this.south, this.southWest, this.west, this.northWest];
    }

    static get noneAndNeighbours(): Direction[]
    {
        return [this.none, this.north, this.northEast, this.east, this.southEast, this.south, this.southWest, this.west, this.northWest];
    }
}