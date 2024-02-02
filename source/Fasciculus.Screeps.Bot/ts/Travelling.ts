import { Dictionaries, Dictionary, DictionaryEntry, Vector } from "./Common";
import { profile } from "./Profiling";
import { Chamber, Chambers } from "./Rooms";

const PATH_MAX_COST: number = 1000000;
const PATHS_MAX_ENTRIES: number = 1000;

export type Positioned = RoomPosition | _HasRoomPosition;

class Path
{
    readonly cost: number;

    accessed: number;

    constructor(pfp: PathFinderPath)
    {
        this.cost = pfp.cost;
        this.accessed = Game.time;
    }
}

export class Paths
{
    private static _paths: Dictionary<Path> = {};

    private static key(s: RoomPosition, g: RoomPosition, range: number): string
    {
        return s.roomName + "_" + s.x + "_" + s.y + "_" + g.roomName + "_" + g.x + "_" + g.y + "_" + range;
    }

    private static positionOf(target: Positioned): RoomPosition
    {
        return target instanceof RoomPosition ? target : target.pos;
    }

    private static callback(name: string | undefined): boolean | CostMatrix
    {
        const chamber: Chamber | undefined = Chambers.get(name);

        return chamber ? chamber.costMatrix : new PathFinder.CostMatrix();
    }

    private static find(start: RoomPosition, goal: RoomPosition, range: number): Path | undefined
    {
        const key = Paths.key(start, goal, range);
        var result: Path | undefined = Paths._paths[key];

        if (result)
        {
            result.accessed = Game.time;
        }
        else
        {
            const opts: PathFinderOpts = { roomCallback: Paths.callback };
            const pfp: PathFinderPath = PathFinder.search(start, { pos: goal, range }, opts);

            if (!pfp.incomplete)
            {
                Paths._paths[key] = result = new Path(pfp);
            }
        }

        return result;
    }

    static cost(start: RoomPosition, goal: RoomPosition, range: number): number
    {
        return Paths.find(start, goal, range)?.cost || PATH_MAX_COST;
    }

    static closest<T extends Positioned>(start: Positioned, goals: Vector<T>, range: number): T | undefined
    {
        const startPos = Paths.positionOf(start);

        return goals.min(goal => Paths.cost(startPos, Paths.positionOf(goal), range));
    }

    @profile
    static cleanup()
    {
        const paths: Dictionary<Path> = Paths._paths;
        const size: number = Dictionaries.size(paths)

        if (size < PATHS_MAX_ENTRIES) return;

        const entries: Vector<DictionaryEntry<Path>> = Dictionaries.entries(paths);
        const toDelete = entries.sort(Paths.older).take(size - PATHS_MAX_ENTRIES);

        for (const entry of toDelete)
        {
            delete paths[entry.key];
        }
    }

    private static older(a: DictionaryEntry<Path>, b: DictionaryEntry<Path>): number
    {
        return a.value.accessed - b.value.accessed;
    }
}