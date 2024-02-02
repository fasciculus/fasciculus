import { Dictionaries, Dictionary, DictionaryEntry, Vector } from "./Common";
import { profile } from "./Profiling";
import { Chamber, Chambers } from "./Rooms";

const PATH_MAX_COST: number = 1000000;
const PATHS_MAX_ENTRIES: number = 1000;

export type Positioned = RoomPosition | _HasRoomPosition;

export class PositionHelper
{
    static positionOf(target: Positioned): RoomPosition
    {
        return target instanceof RoomPosition ? target : target.pos;
    }
}

class Path
{
    readonly cost: number;
    readonly path: Vector<RoomPosition>;

    accessed: number;

    constructor(pfp: PathFinderPath)
    {
        this.cost = pfp.cost;
        this.path = Vector.from(pfp.path);

        this.accessed = Game.time;
    }

    next(pos: RoomPosition): DirectionConstant | 0
    {
        const path: Vector<RoomPosition> = this.path;

        if (path.length == 0) return 0;

        const goal = path.at(0)!;

        return pos.getDirectionTo(goal);
    }

    isBlocked(blocks: Vector<RoomPosition>): boolean
    {
        const path: Vector<RoomPosition> = this.path;

        if (path.length == 0) return false;

        const next: RoomPosition = path.at(0)!;

        for (const pos of blocks)
        {
            if (pos.isEqualTo(next)) return true;
        }

        return false;
    }
}

export class Paths
{
    private static _paths: Dictionary<Path> = {};
    private static _blocks: Dictionary<RoomPosition> = {};

    private static key(s: RoomPosition, g: RoomPosition, range: number): string
    {
        return s.roomName + "_" + s.x + "_" + s.y + "_" + g.roomName + "_" + g.x + "_" + g.y + "_" + range;
    }

    private static callback(name: string | undefined): boolean | CostMatrix
    {
        const chamber: Chamber | undefined = Chambers.get(name);
        var costMatrix: CostMatrix = chamber ? chamber.costMatrix : new PathFinder.CostMatrix();

        if (Dictionaries.size(Paths._blocks))
        {
            costMatrix = costMatrix.clone();

            for (const block of Dictionaries.values(Paths._blocks))
            {
                costMatrix.set(block.x, block.y, 255);
            }
        }

        return costMatrix;
    }

    @profile
    static find(start: RoomPosition, goal: RoomPosition, range: number): Path | undefined
    {
        const key = Paths.key(start, goal, range);
        const paths = Paths._paths;
        var result: Path | undefined = paths[key];

        if (result && result.isBlocked(Dictionaries.values(Paths._blocks)))
        {
            delete paths[key];
            result = undefined;
        }

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

    @profile
    static closest<T extends Positioned>(start: Positioned, goals: Vector<T>, range: number): T | undefined
    {
        const startPos = PositionHelper.positionOf(start);

        return goals.min(goal => Paths.cost(startPos, PositionHelper.positionOf(goal), range));
    }

    static block(creep: Creep)
    {
        Paths._blocks[creep.name] = creep.pos;
    }

    @profile
    static cleanup()
    {
        const paths: Dictionary<Path> = Paths._paths;
        const size: number = Dictionaries.size(paths);

        if (size > PATHS_MAX_ENTRIES)
        {
            const entries: Vector<DictionaryEntry<Path>> = Dictionaries.entries(paths);
            const toDelete = entries.sort(Paths.older).take(size - PATHS_MAX_ENTRIES);

            for (const entry of toDelete)
            {
                delete paths[entry.key];
            }
        }

        const blocks = Paths._blocks;
        const existing: Set<string> = Dictionaries.keys(Game.creeps);

        for (const name in Dictionaries.keys(blocks))
        {
            if (!existing.has(name))
            {
                delete blocks[name];
            }
        }

        const blockCount = Dictionaries.size(blocks);

        console.log(`Path cache has ${size} entries and ${blockCount} blocks.`);
    }

    private static older(a: DictionaryEntry<Path>, b: DictionaryEntry<Path>): number
    {
        return a.value.accessed - b.value.accessed;
    }
}

export class Mover
{
    @profile
    static moveTo(creep: Creep, goal: Positioned, range: number): CreepMoveReturnCode | ERR_NO_PATH
    {
        const start: RoomPosition = creep.pos;
        const path: Path | undefined = Paths.find(start, PositionHelper.positionOf(goal), range);

        if (!path) return ERR_NO_PATH;

        const direction: DirectionConstant | 0 = path.next(start);

        if (direction == 0) return OK;

        return creep.move(direction);
    }
}