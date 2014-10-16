using System;
using System.Collections;
using System.Collections.Generic;

public interface IQuantum : IEnumerable {
	Quantum MakeQuantum ();
}


public abstract class Quantum : IQuantum {

	public Quantum prev;

	public Exception ex;
	
	internal  Quantum () {}
	
	internal Quantum (Quantum prev)
	{
		this.prev = prev;
	}

	public Quantum MakeQuantum () {
		return this;
	}

	public abstract Quantum copyQuantum {get;}
	
	public IEnumerable<Quantum> previousQuanta {
		get {
			return this.IterateWhile (a => a != null, a => a.prev);
		}
	}
	
	public Quantum firstQuantum {
		get {
			return previousQuanta.Last();
		}
	}
	
	public IEnumerable<Quantum> brokenQuanta {
		get {
			return previousQuanta.Filter (a => a.ex != null);
		}
	}
	
	public Maybe<Quantum> firstBrokenQuantum {
		get {
			return brokenQuanta.MaybeLast();
		}
	}
	
	public Maybe<Quantum> lastBrokenQuantum {
		get {
			return brokenQuanta.MaybeHead();
		}
	}
	
	public IEnumerable<Exception> exceptions {
		get {
			return brokenQuanta.FMap (a => a.ex);
		}
	}
	
	public Maybe<Exception> firstException {
		get {
			return exceptions.MaybeLast();
		}
	}
	
	public Maybe<Exception> lastException {
		get {
			return exceptions.MaybeHead();
		}
	}
	
	internal bool valid {
		get {
			return ex != null;
		}
	}
	
	internal IEnumerable Previous () {
		if (prev != null) {
			foreach (var _ in prev) {
				yield return _;	
			}
		}
	}
	
	public IEnumerator GetEnumerator ()
	{
		return GetEnumerable ().GetEnumerator ();
	}
	
	internal abstract IEnumerable GetEnumerable ();

}


public abstract class Atom : Quantum {

	internal  Atom () {}
	
	internal Atom (Quantum prev) : base (prev){}

	public override Quantum copyQuantum {
		get {
			return copyAtom;
		}
	}

	public abstract Atom copyAtom { get; }

	public static Atom operator + (Atom a, Atom b) {
		var copyB = b.copyAtom;
		var copyA = a.copyAtom;

		copyB.firstQuantum.prev = copyA;

		return copyB;
	}

}
