
export class Stores
{
    static energy(target: { store: StoreDefinition }): number
    {
        return target.store.energy;
    }

    static energyCapacity(target: { store: StoreDefinition }): number
    {
        return target.store.getCapacity(RESOURCE_ENERGY);
    }

    static freeEnergyCapacity(target: { store: StoreDefinition }): number
    {
        return target.store.getFreeCapacity(RESOURCE_ENERGY);
    }

    static hasFreeEnergyCapacity(target: { store: StoreDefinition }): boolean
    {
        return target.store.getFreeCapacity(RESOURCE_ENERGY) > 0;
    }

    static energyRatio(target: { store: StoreDefinition }): number
    {
        return Stores.energy(target) / Math.max(1, Stores.energyCapacity(target));
    }
}