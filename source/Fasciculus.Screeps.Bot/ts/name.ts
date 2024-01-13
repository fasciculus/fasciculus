enum NamePrefix
{
    Worker = "W"
}

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
        }
    }

    return result;
}

export class NameManager
{
    static nextWorkerName(): string
    {
        var mem = getNameMemory();

        ++mem.names.worker;

        return NamePrefix.Worker + mem.names.worker;
    }

    static isWorkerName(name: string): boolean
    {
        return name.startsWith(NamePrefix.Worker);
    }
}