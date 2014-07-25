using UnityEngine;
using System;
using System.Collections;

public abstract class DoIf<A> : Functor<A> {

	internal Func<bool> condition = () => false;
	internal Func<bool> guard = () => true;

	public bool truth {
		get {
			return condition() && guard();
		}
	}

	public abstract DoIf<B> FMap<B> (Func<A,B> f);
	public abstract DoIf<A> FMap (Action<A> f);
	public abstract DoIf<A> Try ();
	public abstract DoIf<A> If (Func<bool> cond);
	public abstract DoIf<A> Guard (Func<bool> cond);
	public abstract A value {get;}
	public abstract bool IsWaiting {get;}

	Functor<B> Functor<A>.FMap<B> (Func<A, B> f)
	{
		throw new System.NotImplementedException ();
	}

	public static bool operator true (DoIf<A> m) {
		return !m.IsWaiting;
	}
	
	public static bool operator false (DoIf<A> m) {
		return m.IsWaiting;
	}
	
	public static bool operator ! (DoIf<A> m) {
		return m.IsWaiting;
	}

}

public class Waiting<A> : DoIf<A> {

	Func<A> fa;

	public Waiting (Func<A> f) {
		fa = f;
	}

	public Waiting (A a) {
		fa = () => a;
	}

	public Waiting (Func<A> f, Func<bool> cond) {
		fa = f;
		condition = cond;
	}

	public Waiting (Func<A> f, Func<bool> cond, Func<bool> guard) {
		fa = f;
		condition = cond;
		this.guard = guard;
	}
	
	public Waiting (A a, Func<bool> cond) {
		fa = () => a;
		condition = cond;
	}

	public override DoIf<B> FMap<B> (Func<A, B> f)
	{
		return new Waiting<B> (() => f (fa ()), condition, guard);
	}

	public override DoIf<A> FMap (Action<A> f)
	{
		return new Waiting<A> (() => f.ToFunc () (fa ()), condition, guard);
	}

	public override DoIf<A> If (Func<bool> cond)
	{
		return new Waiting<A> (fa, () => cond() || condition(), guard);
	}

	public override DoIf<A> Guard (Func<bool> cond)
	{
		return new Waiting<A> (fa, condition, () => cond () && guard ());
	}

	public override DoIf<A> Try ()
	{
		return truth ? new Done<A>(fa(), condition, guard) as DoIf<A> : this as DoIf<A>;
	}

	public override A value {
		get {
			return default(A);
		}
	}

	public override bool IsWaiting {
		get {
			return true;
		}
	}
}

public class Done<A> : DoIf<A> {

	A a;

	public Done (A val) {
		a = val;
	}

	public Done (A val, Func<bool> cond) {
		a = val;
		condition = cond;
	}

	public Done (A val, Func<bool> cond, Func<bool> guard) {
		a = val;
		condition = cond;
		this.guard = guard;
	}

	public override DoIf<B> FMap<B> (Func<A, B> f)
	{
		return new Waiting<A> (() => a, condition, guard).FMap (f);
	}

	public override DoIf<A> FMap (Action<A> f)
	{
		return FMap (f.ToFunc ());
	}

	public override DoIf<A> Try ()
	{
		return this;
	}

	public override DoIf<A> If (Func<bool> cond)
	{
		return new Done<A> (a, () => cond () || condition (), guard);
	}

	public override DoIf<A> Guard (Func<bool> cond)
	{
		return new Done<A> (a, condition, () => cond() && guard());
	}

	public override A value {
		get {
			return a;
		}
	}

	public override bool IsWaiting {
		get {
			return false;
		}
	}
	
}

public abstract class DoIf {
	internal Func<bool> condition = () => false;
	
	public bool truth {
		get {
			return condition();
		}
	}
	
	public DoIf FMap<B> (Action f) {
		return new Waiting (f, condition);
	}

	public abstract DoIf Try ();
	public abstract DoIf If (Func<bool> cond);
	public abstract bool IsWaiting {get;}

	public static bool operator true (DoIf m) {
		return ! m.IsWaiting;
	}
	
	public static bool operator false (DoIf m) {
		return m.IsWaiting;
	}
	
	public static bool operator ! (DoIf m) {
		return m.IsWaiting;
	}
}

public class Waiting : DoIf {
	Action f;

	public Waiting (Action g) {
		f = g;
	}

	public Waiting (Action g, Func<bool> cond) {
		f = g;
		condition = cond;
	}

	public override DoIf Try ()
	{
		return truth ? new Done (f) as DoIf : this as DoIf;
	}

	public override DoIf If (Func<bool> cond)
	{
		return new Waiting (f, () => cond () || condition ());
	}

	public override bool IsWaiting {
		get {
			return true;
		}
	}
}

public class Done : DoIf {

	public Done (Func<bool> cond){
		condition = cond;
	}
	public Done (Action f) {
		f ();
	}

	public override DoIf Try ()
	{
		return this;
	}

	public override DoIf If (Func<bool> cond)
	{
		return new Done (() => cond () || condition ());
	}

	public override bool IsWaiting {
		get {
			return false;
		}
	}
}