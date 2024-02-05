import { CreepState, CreepType, Dictionaries, Dictionary, Random, Stores, Vector } from "./Common";
import { CreepBase, CreepBaseMemory, Creeps } from "./Creeps";
import { Spawn, Spawns } from "./Infrastructure";
import { profile } from "./Profiling";
import { Paths } from "./Travelling";
import { Weller, Wellers } from "./Workers";

interface TankerMemory extends CreepBaseMemory
{
    weller?: string;
    customer?: SpawnId;
}

export class Tanker extends CreepBase<TankerMemory>
{
    get weller(): Weller | undefined { return Wellers.get(this.memory.weller); }
    set weller(value: Weller | undefined) { this.memory.weller = value?.name; }

    get customer(): StructureSpawn | undefined { return Game.get<StructureSpawn>(this.memory.customer); }
    set customer(value: Spawn | undefined) { this.memory.customer = value?.id; }

    constructor(name: string)
    {
        super(name);
    }

    prepare()
    {
        switch (this.state)
        {
            case CreepState.Idle: this.state = this.prepareIdle(); break;
            case CreepState.ToSupply: this.state = this.prepareToSupply(); break;
            case CreepState.ToCustomer: this.state = this.prepareToCustomer(); break;
            case CreepState.Withdraw: this.state = this.prepareWithdraw(); break;
            case CreepState.Transfer: this.state = this.prepareTransfer(); break;
        }
    }

    private prepareIdle(): CreepState
    {
        if (this.weller) return CreepState.ToSupply;
        if (this.customer) return CreepState.ToCustomer;

        return CreepState.Idle;
    }

    private prepareToSupply(): CreepState
    {
        const weller = this.weller;

        if (!weller) return CreepState.Idle;

        if (weller.energy == 0)
        {
            this.weller = undefined;
            return CreepState.Idle;
        }

        return this.pos.inRangeTo(weller, 1) ? CreepState.Withdraw : CreepState.ToSupply;
    }

    private prepareToCustomer(): CreepState
    {
        const customer = this.customer;

        if (!customer) return CreepState.Idle;

        if (Stores.freeEnergyCapacity(customer) == 0)
        {
            this.customer = undefined;
            return CreepState.Idle;
        }

        return this.pos.inRangeTo(customer, 1) ? CreepState.Transfer : CreepState.ToCustomer;
    }

    private prepareWithdraw(): CreepState
    {
        if (this.freeEnergyCapacity == 0)
        {
            this.weller = undefined;
            return CreepState.Idle;
        }

        return this.weller ? CreepState.Withdraw : CreepState.Idle
    }

    private prepareTransfer(): CreepState
    {
        if (this.energy == 0)
        {
            this.customer = undefined;
            return CreepState.Idle;
        }

        const customer = this.customer;

        if (!customer) return CreepState.Idle;

        if (Stores.freeEnergyCapacity(customer) == 0)
        {
            this.customer = undefined;
            return CreepState.Idle;
        }

        return CreepState.Transfer
    }

    execute()
    {
        switch (this.state)
        {
            case CreepState.ToSupply: this.executeToSupply(); break;
            case CreepState.ToCustomer: this.executeToCustomer(); break;
            case CreepState.Withdraw: this.executeWithdraw(); break;
            case CreepState.Transfer: this.executeTransfer(); break;
        }
    }

    @profile
    private executeToSupply()
    {
        const weller = this.weller;

        if (!weller) return;

        this.moveTo(weller, 1);
    }

    @profile
    private executeToCustomer()
    {
        const customer = this.customer;

        if (!customer) return;

        this.moveTo(customer, 1);
    }

    @profile
    private executeWithdraw()
    {
        const weller = this.weller;

        if (!weller) return;

        const amount = Math.min(this.freeEnergyCapacity, Math.max(0, weller.energy - 1));

        if (amount == 0) return;

        this.withdraw(weller.creep, RESOURCE_ENERGY, amount);
    }

    @profile
    private executeTransfer()
    {
        const customer = this.customer;

        if (!customer) return;

        this.transfer(customer, RESOURCE_ENERGY);
    }
}

export class Tankers
{
    private static _tankers: Map<string, Tanker> = new Map();
    private static _all: Array<Tanker> = new Array();

    static get count(): number { return Tankers._all.length; }

    @profile
    static initialize()
    {
        if (Creeps.update(Tankers._tankers, CreepType.Tanker, name => new Tanker(name)))
        {
            Tankers._all = Tankers._tankers.vs();
        }
    }

    static run()
    {
        Tankers.prepare(Tankers._all);
        Tankers.prepare(Tankers.assign());
        Tankers.execute(Tankers._all);
    }

    @profile
    private static prepare(tankers: Array<Tanker>)
    {
        tankers.forEach(t => t.prepare());
    }

    private static execute(tankers: Array<Tanker>)
    {
        tankers.forEach(t => t.execute());
    }

    @profile
    private static assign(): Array<Tanker>
    {
        const result: Array<Tanker> = new Array();
        const unassigned: Array<Tanker> = Tankers._all.filter(t => !t.spawning && t.state == CreepState.Idle);

        if (unassigned.length == 0) return result;

        const empty: Array<Tanker> = new Array();
        const full: Array<Tanker> = new Array();

        Tankers.categorize(unassigned, empty, full);

        if (empty.length > 0)
        {
            var wellers: Vector<Weller> = Wellers.all;

            if (wellers.length > 0)
            {
                const weller: Weller = wellers.sort(Tankers.compareWellers).at(0)!;
                const tanker: Tanker | undefined = Paths.closest(weller, empty, 1);

                if (tanker)
                {
                    tanker.weller = weller;
                    result.push(tanker);
                }
            }
        }

        if (full.length > 0)
        {
            var customers: Vector<Spawn> = Vector.from(Spawns.all.filter(s => s.freeEnergyCapacity > 0));

            if (customers.length > 0)
            {
                customers = customers.sort(Tankers.compareCustomers).take(3);

                const customer: Spawn = customers.at(Random.nextInt(customers.length))!;
                const tanker: Tanker | undefined = Paths.closest(customer, full, 1);

                if (tanker)
                {
                    tanker.customer = customer;
                    result.push(tanker);
                }
            }
        }

        return result;
    }

    private static categorize(tankers: Array<Tanker>, empty: Array<Tanker>, full: Array<Tanker>)
    {
        for (const tanker of tankers)
        {
            if (tanker.energy == 0)
            {
                empty.push(tanker);
            }
            else
            {
                full.push(tanker);
            }
        }
    }

    private static compareWellers(a: { energy: number }, b: { energy: number }): number
    {
        return b.energy - a.energy;
    }

    private static compareCustomers(a: { freeEnergyCapacity: number }, b: { freeEnergyCapacity: number }): number
    {
        return b.freeEnergyCapacity - a.freeEnergyCapacity;
    }
}