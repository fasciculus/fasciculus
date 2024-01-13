export class CreepManager
{
    static newName(prefix: string): string
    {
        var index: number = 1;
        var name: string = `${prefix}${index}`;

        while (Game.creeps[name])
        {
            ++index;
            name = `${prefix}${index}`;
        }

        return name;
    }
}