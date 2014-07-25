using UnityEngine;
using System;
using System.Collections;

public class Tuple<A,B> : Functor<B> {

	public readonly A fst;
	public readonly B snd;

	public Tuple (A a, B b) {
		fst = a;
		snd = b;
	}

	Functor<C> Functor<B>.FMap<C> (Func<B, C> f)
	{
		return new Tuple<A,C> (fst, f (snd));
	}
}

public static partial class Fn {
	public static Tuple<A,B> Tuple<A,B> (A a, B b) {
		return new Tuple<A,B> (a, b);
	}
}
