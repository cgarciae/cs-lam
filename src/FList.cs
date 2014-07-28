using UnityEngine;
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

public static partial class Fn {

	//Functor
	//FMap :: (a -> b) -> IEnumerable a -> IEnumerable b
	public static IEnumerable<B> FMap<A,B> (Func<A,B> f, IEnumerable<A> F) {
		return F.Select (f);
	}

	//FMap :: (a -> b) -> (IEnumerable a -> IEnumerable b)
	public static Func<IEnumerable<A>,IEnumerable<B>> FMap<TList,A,B> (Func<A,B> f) {
		return F => F.Select (f);
	}

	//Applicative
	//Pure :: a -> [a]
	public static IEnumerable<A> Pure<A> (TList _, A a) {
		return Fn.Repeat (a);
	}

	//Pure :: a -> [a]
	public static IEnumerable<A> Pure<A> (this IEnumerable<A> l, A a) {
		return Fn.Repeat (a);
	}
}

public static partial class Fn {

	// Map :: (a -> b) -> [a] -> [b]
	public static List<B> Map<A,B> (Func<A,B> f, List<A> e) {
		return FMap (f, e).ToList ();
	}
	// Map :: (a -> b) -> ([a] -> [b])
	public static Func<List<A>,List<B>> Map<A,B> (Func<A,B> f) {
		return e => FMap (f, e).ToList ();
	}
	// Map :: ((a -> b) -> ([a] -> [b]))
	public static Func<Func<A,B>,Func<List<A>,List<B>>> Map<A,B> () {
		return f => e => FMap (f, e).ToList ();
	}

	// Reverse :: [a] -> [a]
	public static IEnumerable<A> Reverse<A> (IEnumerable<A> e) {
		return e.Reverse ();
	}

	// Reverse :: ([a] -> [a])
	public static Func<IEnumerable<A>,IEnumerable<A>> Reverse<A> () {
		return e => e.Reverse ();
	}

	// FoldL :: (a -> b -> a) -> a -> [b] -> a
	public static A FoldL<A,B> (Func<A,B,A> f, A a, IEnumerable<B> e) {
		return e.Aggregate (a, f);
	}

	// FoldL :: (a -> b -> a) -> a -> ([b] -> a)
	public static Func<IEnumerable<B>,A> FoldL<A,B>  (Func<A,B,A> f, A a) {
		return e => FoldL (f, a, e);
	}

	// FoldL :: (a -> b -> a) -> (a -> ([b] -> a))
	public static Func<A,Func<IEnumerable<B>,A>> FoldL<A,B>  (Func<A,B,A> f) {
		return a => e => FoldL (f, a, e);
	}

	// FoldL :: ((a -> b -> a) -> (a -> ([b] -> a)))
	public static Func<Func<A,B,A>,Func<A,Func<IEnumerable<B>,A>>> FoldL<A,B>  () {
		return f => a => e => FoldL (f, a, e);
	}

	// ZipWith :: (a -> b -> c) -> [a] -> [b] -> [c]
	public static IEnumerable<C> ZipWith<A,B,C> (Func<A,B,C> f, IEnumerable<A> ea, IEnumerable<B> eb) {
		var a = ea.GetEnumerator ();
		var b = eb.GetEnumerator ();

		while (b.MoveNext() && a.MoveNext()) {
			yield return f (a.Current, b.Current);
		}
	}

	// ZipWith :: (a -> b -> c) -> ([a] -> ([b] -> [c]))
	public static Func<IEnumerable<A>,Func<IEnumerable<B>,IEnumerable<C>>> ZipWith<A,B,C> (Func<A,B,C> f) {
		return ea => eb => ZipWith (f, ea, eb);
	}

	// ZipWith :: ((a -> b -> c) -> ([a] -> ([b] -> [c])))
	public static Func<Func<A,B,C>,Func<IEnumerable<A>,Func<IEnumerable<B>,IEnumerable<C>>>> ZipWith<A,B,C> () {
		return f => ea => eb => ZipWith (f, ea, eb);
	}

	// ZipWith :: (a -> b -> c) -> [a] -> [b] -> [c]
	public static IEnumerable<B> ZipWith<A,B> (Action<A,B> f, IEnumerable<A> ea, IEnumerable<B> eb) {
		return ZipWith (f.ToFunc (), ea, eb);  
	}

	// ScanL :: :: (a -> a -> a) -> [a] -> [a]
	public static IEnumerable<A> ScanL1<A> (Func<A,A,A> f, IEnumerable<A> e) {
		A a;

		var num = e.GetEnumerator ();
		num.MoveNext ();
		yield return (a = num.Current);

		while (num.MoveNext()) {
			yield return (a = f (a, num.Current));
		}
	}

	// ScanL :: :: (a -> b -> a) -> a -> [b] -> a
	public static IEnumerable<A> ScanL<A,B> (Func<A,B,A> f, A a, IEnumerable<B> e) {
		yield return a;
		foreach (var b in e) {
			yield return (a = f (a, b));
		}
	}

	// Repeat :: a -> [a]
	// a.Repeat :: [a]
	public static IEnumerable<A> Repeat<A> (this A a) {
		while (true) yield return a;
		Pure (TMaybe.i, 1);
		Fn.MakeMaybe (1);
	}

	// Take :: int -> [a] -> [a]
	public static IEnumerable<A> Take<A> (int n, IEnumerable<A> e) {
		var num = e.GetEnumerator ();
		
		var i = -1;
		while (++i < n && num.MoveNext()) {
			yield return num.Current;
		}
	}

	// Take :: int -> ([a] -> [a])
	public static Func<IEnumerable<A>,IEnumerable<A>> Take<A> (int n) {
		return e => Take (n, e);
	}

	// [a].Take :: n -> IEnumerable a 
	public static IEnumerable<A> Take<A> (this IEnumerable<A> e, int n) {
		return Take (n, e);
	}

	// Iterate :: (a -> a) -> a -> [a]
	public static IEnumerable<A> Iterate<A> (Func<A,A> f, A a) {
		while (true) {
			yield return a;
			a = f(a);
		}
	}

	// Iterate :: (a -> a) -> (a -> [a])
	public static Func<A,IEnumerable<A>> Iterate<A> (Func<A,A> f) {
		return a => Iterate (f, a);
	}

	// a.Iterate :: (a -> a) -> [a]
	public static IEnumerable<A> Iterate<A> (this A a, Func<A,A> f) {
		return Iterate (f, a);
	}

	public static IEnumerable<A> Cycle<A> (IEnumerable<A> e){
		while (true)
			foreach(var a in e) 
				yield return a;
	}

	public static Func<IEnumerable<A>,IEnumerable<A>> Cycle<A> () {
		return e => Cycle (e);
	}
}

public class TList {
	private static TList _i;
	public static TList i {
		get {
			if (_i != null) _i = new TList();
			return _i;
		}
	}
	private TList (){}
}
