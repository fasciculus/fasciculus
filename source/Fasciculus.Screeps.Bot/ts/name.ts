
interface INameMemory
{
    names:
    {
        worker: number
    }
}

function getNameMemory(): INameMemory
{
    var result = Memory as any as INameMemory;

    if (!result.names)
    {
        result.names =
        {
            worker: 0
        };
    }

    return result;
}

export function nextWorkerName(): string
{
    let mem = getNameMemory();
    let id = mem.names.worker + 1;

    mem.names.worker = id;

    return `W${id}`;
}