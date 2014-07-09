using UnityEngine;
using System;
using System.Collections.Generic;
using System.Collections;
using System.Linq;

public abstract class Promise<A,B> : Functor<B> {

	public abstract Promise<A,C> FMap<C> (Func<B,C> g);
	public abstract Promise<A,B> FMap (Func<B,B> g);
	public abstract Promise<A,B> FMap (Action<B> g);

	public abstract Promise<A,B> Resolve (A val);
	public abstract Promise<A,B> Reject();

	public abstract B value { get; }

	Functor<C> Functor<B>.FMap<C> (Func<B, C> f) {
		return FMap (f);
	}

	Functor<B> Functor<B>.FMap (Action<B> f) {
		return FMap (f);
	}

}

public class Pending<A,B> : Promise<A,B> {

	Func<A,B> f;

	public Pending (Func<A,B> g) {
		f = g;	
	}

	public override Promise<A,C> FMap<C> (Func<B, C> g)
	{
		return new Pending<A,C> (Fn.Compose (g, f));
	}

	public override Promise<A,B> FMap (Func<B, B> g)
	{
		f = Fn.Compose (g, f);
		return this;
	}

	public override Promise<A,B> FMap (Action<B> g)
	{
		f = Fn.Compose (g.ToFunc (), f);
		return this;
	}

	public override Promise<A,B> Resolve (A val)
	{
		return new Resolved<A,B> (f (val));
	}

	public override Promise<A,B> Reject ()
	{
		return new Broken<A,B> ();
	}

	public override B value {
		get {
			return default(B);
		}
	}
}

public class Resolved<A,B> : Promise<A,B> {

	B val;

	public Resolved (B value) {
		val = value;	
	}

	public override Promise<A,C> FMap<C> (Func<B, C> g)
	{
		return new Resolved<A,C> (g (val));
	}

	public override Promise<A,B> FMap (Func<B, B> g)
	{
		val = g (val);
		return this;
	}

	public override Promise<A,B> FMap (Action<B> g)
	{
		g (val);
		return this;
	}

	public override Promise<A,B> Resolve (A val)
	{
		return this;
	}

	public override Promise<A,B> Reject ()
	{
		return this;
	}

	public override B value {
		get {
			return val;
		}
	}
}

public class Broken<A,B> : Promise<A,B> {

	public override Promise<A,C> FMap<C> (Func<B, C> g)
	{
		return new Broken<A,C> ();
	}

	public override Promise<A,B> FMap (Func<B, B> g)
	{
		return this;
	}

	public override Promise<A,B> FMap (Action<B> g)
	{
		return this;
	}

	public override Promise<A,B> Resolve (A val)
	{
		return this;
	}

	public override Promise<A,B> Reject ()
	{
		return this;
	}

	public override B value {
		get {
			return default(B);
		}
	}
}

public interface Promise {
	
	Promise FMap (Action g);
	Promise resolve ();
	Promise reject ();
}

public class Pending : Promise {
	Action f;

	public Pending (Action g) {
		f = g;
	}

	public Promise FMap (Action g)
	{
		f = Fn.Compose (g, f);
		return this;
	}

	public Promise resolve ()
	{
		f ();
		return new Resolved ();
	}

	public Promise reject ()
	{
		return new Broken ();
	}
}

public class Resolved : Promise {

	public Promise FMap (Action g)
	{
		g ();
		return this;
	}

	public Promise resolve ()
	{
		return this;
	}

	public Promise reject ()
	{
		return this;
	}

}

public class Broken : Promise {

	public Promise FMap (Action g)
	{
		return this;
	}

	public Promise resolve ()
	{
		return this;
	}

	public Promise reject ()
	{
		return this;
	}
}

public static partial class Fn {

	//FMap :: (a -> b) -> (Maybe a -> Maybe b)
	public static Func<Promise<R,A>,Promise<R,B>> FMap<Promise,R,A,B> (Func<A,B> f) {
		return F => F.FMap (f);
	}

	//FMap :: (a -> void) -> (Maybe a -> Maybe a)
	public static Func<Promise<R,A>,Promise<R,A>> FMap<Promise,R,A> (Action<A> f) {
		return F => F.FMap (f);
	}

	//FMap :: (a -> void) -> (Maybe a -> Maybe a)
	public static Func<Promise<A,A>,Promise<A,A>> FMap<Promise,A> (Action<A> f) {
		return F => F.FMap (f);
	}
}
