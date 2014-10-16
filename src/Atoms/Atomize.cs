using System;
using System.Collections;

public class Atomize<A> : Chain<A> {

	IChain<A> chain;

	public Atomize (IChain<A> chain)
	{
		this.chain = chain;
	}

	public Atomize (IChain<A> chain, Quantum prev) : base (prev)
	{
		this.chain = chain;
	}

	internal override IEnumerable GetEnumerable ()
	{
		return chain;
	}

	public override Chain<A> copyChain {
		get {
			return new Atomize<A> (chain, prev);
		}
	}

}
