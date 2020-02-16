using System.Collections.Generic;
using JReact.Tilemaps;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace System
{
    public abstract class J_TokenOnTile<T, K> : ScriptableObject
        where K : J_Tile
    {
        // --------------- EVENTS --------------- //
        public Action<(T token, K tile)> OnTokenPlaced;
        public Action<(T token, K tile)> OnTokenRemoved;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<T, K> _tokenToTile = new Dictionary<T, K>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] private Dictionary<K, T> _tileToToken = new Dictionary<K, T>();

        // --------------- QUERIES --------------- //
        public T GetTokenOnTile(K   tile)  => !_tileToToken.ContainsKey(tile) ? default : _tileToToken[tile];
        public K GetTileFromToken(T token) => !_tokenToTile.ContainsKey(token) ? default : _tokenToTile[token];

        public virtual bool IsTileFree(K tile)
        {
            if (!_tileToToken.ContainsKey(tile)) return true;
            else return _tileToToken[tile] == null;
        }

        public bool IsTokenOnBoard(T token) => _tokenToTile.ContainsKey(token);

        // --------------- COMMANDS --------------- //
        public void PlaceTokenOnTile(T token, K tile)
        {
            Assert.IsTrue(IsTileFree(tile), $"{name} - {tile} contains {GetTokenOnTile(tile)}. Cannot place {token}");
            _tileToToken[tile]  = token;
            _tokenToTile[token] = tile;
            OnTokenPlaced?.Invoke((token, tile));
        }

        public void RemoveToken(T token)
        {
            var tile = _tokenToTile[token];
            Assert.IsTrue(_tokenToTile.ContainsKey(token), $"{name} {token} is not on board.");
            _tileToToken.Remove(tile);
            _tokenToTile.Remove(token);
            OnTokenRemoved?.Invoke((token, tile));
        }

        public void FreeTile(K tile)
        {
            var token = _tileToToken[tile];
            Assert.IsTrue(_tileToToken.ContainsKey(tile), $"{name} {tile} is not tracked.");
            _tokenToTile.Remove(token);
            _tileToToken.Remove(tile);
            OnTokenRemoved?.Invoke((token, tile));
        }

        public void ResetThis()
        {
            _tokenToTile.Clear();
            _tileToToken.Clear();
        }
    }
}
