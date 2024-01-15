
export class BotCapabilities
{
    readonly canMove: boolean = false;
    readonly canWork: boolean = false;
    readonly canCarry: boolean = false;

    readonly hasFreeCapacity: boolean;
    readonly hasEnergy: boolean;

    readonly canHarvest: boolean;
    readonly canUpgrade: boolean;
    readonly canSupply: boolean;
    readonly canBuild: boolean;

    constructor(creep: Creep)
    {
        for (var part of creep.body)
        {
            if (part.hits == 0) continue;

            switch (part.type)
            {
                case MOVE: this.canMove = true; break;
                case WORK: this.canWork = true; break;
                case CARRY: this.canCarry = true; break;
            }
        }

        var store = creep.store;

        this.hasFreeCapacity = store.getFreeCapacity() > 0;
        this.hasEnergy = store.energy > 0;

        this.canHarvest = this.canMove && this.canWork && this.canCarry && this.hasFreeCapacity;
        this.canUpgrade = this.canMove && this.canWork && this.canCarry && this.hasEnergy;
        this.canSupply = this.canMove && this.canCarry && this.hasEnergy;
        this.canBuild = this.canMove && this.canWork && this.canCarry && this.hasEnergy;
    }
}