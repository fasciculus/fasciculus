import { Dictionary, Vector, Vectors } from "./Common";

export const DIRECTIONS: Vector<DirectionConstant> = Vector.from([TOP, TOP_RIGHT, RIGHT, BOTTOM_RIGHT, BOTTOM, BOTTOM_LEFT, LEFT, TOP_LEFT]);

const DIRECTION_STRINGS: string[] = ["", "1", "2", "3", "4", "5", "6", "7", "8"];

const DIRECTION_DICTIONARY: Dictionary<DirectionConstant> =
{
    "1": TOP,
    "2": TOP_RIGHT,
    "3": RIGHT,
    "4": BOTTOM_RIGHT,
    "5": BOTTOM,
    "6": BOTTOM_LEFT,
    "7": LEFT,
    "8": TOP_LEFT
}

export class Directions
{
    static encode(directions: Vector<DirectionConstant>): string
    {
        let result: string = "";

        directions.forEach(d => result += DIRECTION_STRINGS[d]);

        return result;
    }

    static decode(value?: string): Vector<DirectionConstant>
    {
        return Vectors.defined(Vector.from(Array.from(value || "")).map(s => DIRECTION_DICTIONARY[s]));
    }
}

export class Point
{
    readonly x: number;
    readonly y: number;

    constructor(x: number, y: number)
    {
        this.x = x;
        this.y = y;
    }

    static from(pos: RoomPosition): Point
    {
        return new Point(pos.x, pos.y);
    }

    around(): Vector<Point>
    {
        let x = this.x;
        let y = this.y;

        let top = new Point(x, y - 1);
        let topRight = new Point(x + 1, y - 1);
        let right = new Point(x + 1, y);
        let bottomRight = new Point(x + 1, y + 1);
        let bottom = new Point(x, y + 1);
        let bottomLeft = new Point(x - 1, y + 1);
        let left = new Point(x - 1, y);
        let topLeft = new Point(x - 1, y - 1);

        return Vector.from([this, top, topRight, right, bottomRight, bottom, bottomLeft, left, topLeft]);
    }
}