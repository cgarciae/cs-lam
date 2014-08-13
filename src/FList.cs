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

	//FMap :: (a -> void) -> IEnumerable a -> IEnumerable b
	public static IEnumerable<A> FMap<A> (Action<A> f, IEnumerable<A> F) {
		return F.Select (f.ToFunc());
	}

	//FMap :: (a -> b) -> (IEnumerable a -> IEnumerable b)
	public static Func<IEnumerable<A>,IEnumerable<A>> FMap<TList,A> (Action<A> f) {
		return F => FMap (f, F);
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

	// Map :: (a -> b) -> [a] -> [b]
	public static List<B> Map<A,B> (this List<A> e, Func<A,B> f) {
		return Map (f, e);
	}

	// Map :: (a -> void) -> [a] -> [a]
	public static List<A> Map<A> (Action<A> f, List<A> e) {
		return Map (f.ToFunc(), e);
	}
	// Map :: (a -> void) -> ([a] -> [a])
	public static Func<List<A>,List<A>> Map<A> (Action<A> f) {
		return e => Map (f, e);
	}
	// Map :: ((a -> void) -> ([a] -> [a]))
	public static Func<Action<A>,Func<List<A>,List<A>>> Map<A> () {
		return f => e => Map (f, e);
	}
	// Map :: (a -> void) -> [a] -> [a]
	public static List<A> Map<A> (this List<A> e, Action<A> f) {
		return Map (f, e);
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
		while (true)
			yield return a;
	}

	public static IEnumerable<A> Repeat<A> (this Func<A> f) {
		while (true)
			yield return f();
	}

	public static IEnumerable Repeat (this Action f) {
		while (true) {
			f();
			yield return null;
		}
			
	}



	// Take :: int -> [a] -> [a]
	public static IEnumerable<A> Take<A> (int n, IEnumerable<A> e) {
		return e.Take (n);
	}

	// Take :: int -> ([a] -> [a])
	public static Func<IEnumerable<A>,IEnumerable<A>> Take<A> (int n) {
		return e => Take (n, e);
	}

	// Take :: (int -> ([a] -> [a]))
	public static Func<int,Func<IEnumerable<A>,IEnumerable<A>>> Take<A> () {
		return n => e => Take (n, e);
	}

	// Drop :: int -> [a] -> [a]
	public static IEnumerable<A> Drop<A> (int n, IEnumerable<A> e) {
		return e.Skip (n);
	}
	
	// Take :: int -> ([a] -> [a])
	public static Func<IEnumerable<A>,IEnumerable<A>> Drop<A> (int n) {
		return e => Drop (n, e);
	}
	
	// Take :: (int -> ([a] -> [a]))
	public static Func<int,Func<IEnumerable<A>,IEnumerable<A>>> Drop<A> () {
		return n => e => Drop (n, e);
	}

	// Drop :: [a] -> int -> [a]
	public static IEnumerable<A> Drop<A> (this IEnumerable<A> e, int n) {
		return e.Skip (n);
	}

	// Slice :: int -> int -> [a] -> [a]
	public static IEnumerable<A> Slice<A> (int from, int to, IEnumerable<A> e) {
		return e.Take (to).Skip (from);
	}

	// Slice :: int -> (int -> ([a] -> [a]))
	public static Func<int,Func<IEnumerable<A>,IEnumerable<A>>> Slice<A> (int from) {
		return to => e => Slice(from, to, e);
	}

	// Slice :: (int -> (int -> ([a] -> [a])))
	public static Func<int,Func<int,Func<IEnumerable<A>,IEnumerable<A>>>> Slice<A> () {
		return from => to => e => Slice(from, to, e);
	}

	// Slice :: int -> int -> [a] -> [a]
	public static IEnumerable<A> Slice<A> (this IEnumerable<A> e, int from, int to) {
		return Slice (from, to, e);
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

	//Join :: [a] -> [a] -> [a]
	public static IEnumerable<A> Join<A> (IEnumerable<A> a, IEnumerable<A> b) {
		var enuA = a.GetEnumerator ();
		var enuB = b.GetEnumerator ();

		while (enuA.MoveNext())
			yield return enuA.Current;

		while (enuB.MoveNext())
			yield return enuB.Current;
	}

	//Join :: [a] -> [a] -> [a]
	public static Func<IEnumerable<A>,IEnumerable<A>> Join<A> (IEnumerable<A> a) {
		return b => Join (a, b);
	}

	//Join :: [a] -> [a] -> [a]
	public static Func<IEnumerable<A>,Func<IEnumerable<A>,IEnumerable<A>>> Join<A> () {
		return a => b => Join (a, b);
	}

	//Join :: [a] -> [a] -> [a]
	public static IEnumerable Join (this IEnumerable a, IEnumerable b) {
		var enuA = a.GetEnumerator ();
		var enuB = b.GetEnumerator ();
		
		while (enuA.MoveNext())
			yield return enuA.Current;
		
		while (enuB.MoveNext())
			yield return enuB.Current;
	}

	//Join :: [a] -> [a] -> [a]
	public static Func<IEnumerable,IEnumerable> Join (IEnumerable a) {
		return b => Join (a, b);
	}

	//Join :: [a] -> [a] -> [a]
	public static Func<IEnumerable,Func<IEnumerable,IEnumerable>> Join () {
		return a => b => Join (a, b);
	}

	public static IEnumerable Then (this IEnumerable a, IEnumerable b) {
		return Join (a, b);
	}

	public static IEnumerable Then (this IEnumerable a, Action f) {
		return Join (a, PureEnumeble (f));
	}

	public static IEnumerable PureEnumeble (Action f) {
		f ();
		yield return null;
	}

	public static Func<Action,IEnumerable> PureEnumeble () {
		return f => PureEnumeble (f);
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
