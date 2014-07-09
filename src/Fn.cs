using UnityEngine;
using System;
using System.Collections;

public static partial class Fn {

	//ToFunc :: (a -> void) -> (a -> a)
	public static Func<A, A> ToFunc<A>(this Action<A> act)
	{
		return x => { 
			act(x); 
			return x; 
		};
	}

	//ToFunc :: (void -> void) -> (a -> a)
	public static Func<A, A> ToFunc<A>(this Action act)
	{
		return x => { 
			act(); 
			return x; 
		};
	}

	//COMPOSE Func :: (b -> c) -> (a -> b) -> (a -> c)
	public static Func<A,C> Compose<A,B,C> ( Func<B,C> g, Func<A,B> f ) {
		return x => g (f (x));
	}
	
	public static Func<A,D> Compose<A,B,C,D> ( Func<C,D> h, Func<B,C> g, Func<A,B> f ) {
		return Compose (Compose (h, g), f);
	}
	
	public static Func<A,E> Compose<A,B,C,D,E> ( Func<D,E> i, Func<C,D> h, Func<B,C> g, Func<A,B> f ) {
		return Compose (Compose (i, h, g), f);
	}

	//Curried Compose :: (b -> c) -> ((a -> b) -> (a -> c))
	public static Func<Func<A,B>,Func<A,C>> Compose<A,B,C> (Func<B,C> g) {
		return f => Compose(g, f);
	}
	


	//PIPE Func :: (a -> b) -> (b -> c) -> (a -> c)
	public static Func<A,C> Pipe<A,B,C> ( Func<A,B> f, Func<B,C> g ) {
		return Compose (g, f);
	}

	public static Func<A,D> Pipe<A,B,C,D> (Func<A,B> f, Func<B,C> g, Func<C,D> h) {
		return Compose (Compose (h, g), f);
	}
	
	public static Func<A,E> Compose<A,B,C,D,E> (Func<A,B> f, Func<B,C> g, Func<C,D> h, Func<D,E> i) {
		return Compose (Compose (i, h, g), f);
	}

	//Curried Pipe :: (a -> b) -> ((b -> c) -> (a -> c))
	public static Func<Func<B,C>,Func<A,C>> Pipe<A,B,C> ( Func<A,B> f ) {
		return g => Compose (g, f);
	}
	

	//COMPOSE Action<A> :: (a -> void) -> (a -> void) -> (a -> void)
	public static Action<A> Compose<A>( Action<A> g, Action<A> f ){
		return a => {
			f(a);
			g(a);
		};
	}
	
	public static Action<A> Compose<A>( Action<A> h, Action<A> g, Action<A> f ){
		return Compose (Compose (h, g), f);
	}
	
	public static Action<A> Compose<A>( Action<A> i, Action<A> h, Action<A> g, Action<A> f ){
		return Compose (Compose (i, h, g), f);
	}
	 
	//PIPE Action<A> :: (a -> void) -> (a -> void) -> (a -> void)
	public static Action<A> Pipe<A> (Action<A> f, Action<A> g){
		return Compose (f, g);
	}
	
	public static Action<A> Pipe<A> (Action<A> f, Action<A> g, Action<A> h){
		return Compose (Compose (h, g), f);
	}
	
	public static Action<A> Pipe<A> (Action<A> f, Action<A> g, Action<A> h, Action<A> i){
		return Compose (Compose (i, h, g), f);
	}

	//Curried PIPE Action<A> :: (a -> void) -> ((a -> void) -> (a -> void))
	public static Func<Action<A>,Action<A>> Pipe<A> (Action<A> f){
		return g => Compose (f, g);
	}

	//COMPOSE Action :: (void -> void) -> (void -> void) -> (void -> void)
	public static Action Compose (Action g, Action f) {
		return Compose (g, f);
	}
	
	public static Action Compose (Action h, Action g, Action f) {
		return Compose (Compose (h, g), f);
	}
	
	public static Action Compose (Action i, Action h, Action g, Action f) {
		return Compose (Compose (i, h, g), f);
	}

	//COMPOSE Action :: (void -> void) -> ((void -> void) -> (void -> void))
	public static Func<Action,Action> Compose (Action g) {
		return f => Compose (g, f);
	}

	//PIPE Action :: (void -> void) -> (void -> void) -> (void -> void)
	public static Action Pipe (Action f, Action g) {
		return Compose (g, f);
	}
	
	public static Action Pipe (Action f, Action g, Action h) {
		return Compose (Compose (h, g), f);
	}
	
	public static Action Pipe (Action f, Action g, Action h, Action i) {
		return Compose (Compose (i, h, g), f);
	}

	//PIPE Action :: (void -> void) -> ((void -> void) -> (void -> void))
	public static Func<Action,Action> Pipe (Action f) {
		return g => Compose (g, f);
	}

	//OF
	public static Func<A,C> Of<A,B,C> (this Func<B,C> f, Func<A,B> g) {
		return x => f (g (x));	
	}

	public static Func<A,B> Of<A,B> (this Func<A,B> f, Action<A> g) {
		return f.Of (g.ToFunc ());	
	}

	public static Func<A,B> Of<A,B> (this Action<B> f, Func<A,B> g) {
		return f.ToFunc ().Of (g);
	}

	public static Action<A> Of<A> (this Action<A> f, Action<A> g) {
		return x => {
			g(x);
			f(x);
		};
	}

	public static Action Of (this Action f, Action g) {
		return () => {
			g();
			f();
		};
	}

	//THEN
	public static Func<A,C> Then<A,B,C> (this Func<A,B> f, Func<B,C> g) {
		return g.Of (f);	
	}
	
	public static Func<A,B> Then<A,B> (this Func<A,B> f, Action<B> g) {
		return g.Of (f);	
	}
	
	public static Func<A,B> Then<A,B> (this Action<A> f, Func<A,B> g) {
		return g.Of (f);
	}
	
	public static Action<A> Then<A> (this Action<A> f, Action<A> g) {
		return g.Of (f);
	}
	
	public static Action Then<A> (Action f, Action g) {
		return g.Of (f);
	}

	//Curry :: (a -> b -> c) -> (a -> (b -> c))
	public static Func<A,Func<B,C>> Curry<A,B,C> (this Func<A,B,C> f) {
		return a => b => f (a, b);
	}

	public static Func<A,Func<B,Func<C,D>>> Curry<A,B,C,D> (this Func<A,B,C,D> f) {
		return a => b => c => f (a, b, c);
	}

	public static Func<A,Func<B,Func<C,Func<D,E>>>> Curry<A,B,C,D,E> (this Func<A,B,C,D,E> f) {
		return a => b => c => d => f (a, b, c, d);
	}

	//FMap :: (a -> b) -> F a -> F b
	public static Functor <B> FMap<A,B> (Func<A,B> f, Functor <A> F) {
		return F.FMap (f);
	}

	//FMap :: (a -> void) -> F a -> F a
	public static Functor <A> FMap<A> (Action<A> f, Functor <A> F) {
		return F.FMap (f);
	}

	//FMap :: (a -> b) -> (F a -> F b)
	public static Func<Functor<A>,Functor<B>> FMap<A,B> (Func<A,B> f) {
		//return Curry ((Func<Func<A,B>,Functor<A>,Functor<B>>)FMap<A,B>) (f);
		return F => FMap (f, F);
	}

	//FMap :: (a -> b) -> (F a -> F b)
	public static Func<Functor<A>,Functor<A>> FMap<A> (Action<A> f) {
		//return Curry ((Func<Func<A,B>,Functor<A>,Functor<B>>)FMap<A,B>) (f);
		return F => FMap (f, F);
	}

}

//Interfaces
public interface Functor<A> {
	Functor<B> FMap<B> (Func<A,B> f);
	Functor<A> FMap (Action<A> f);
}
