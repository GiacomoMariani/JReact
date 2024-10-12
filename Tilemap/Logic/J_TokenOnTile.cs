using System;
using System.Collections.Generic;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace JReact.Tilemaps.Logic
{
    public abstract class J_TokenOnTile<TToken> : MonoBehaviour
    {
        // --------------- EVENTS --------------- //
        public Action<(TToken token, JTile tile)> OnTokenPlaced;
        public Action<(TToken token, JTile tile)> OnTokenRemoved;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected Dictionary<TToken, JTile> _tokenToTile = new Dictionary<TToken, JTile>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected Dictionary<JTile, TToken> _tileToToken = new Dictionary<JTile, TToken>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected List<TToken> _allTokens = new List<TToken>(50);
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] public int TokenOnBoard => _allTokens?.Count ?? 0;

        // --------------- QUERIES --------------- //
        public TToken GetTokenOnTile(JTile tile) => !_tileToToken.ContainsKey(tile)
                                                        ? default
                                                        : _tileToToken[tile];

        public JTile GetTileFromToken(TToken token) => !_tokenToTile.ContainsKey(token)
                                                           ? default
                                                           : _tokenToTile[token];

        public virtual bool IsTileFree(JTile tile)
        {
            if (!_tileToToken.TryGetValue(tile, out TToken value)) { return true; }
            else { return value == null; }
        }

        public bool IsTokenOnBoard(TToken token) => _tokenToTile.ContainsKey(token);

        public TToken GetTokenFromIndex(int index) => _allTokens[index];

        // --------------- COMMANDS --------------- //
        public void PlaceTokenOnTile(TToken token, JTile tile)
        {
            Assert.IsTrue(IsTileFree(tile), $"{name} - {tile} contains {GetTokenOnTile(tile)}. Cannot place {token}");
            if (_tokenToTile.ContainsKey(token))
            {
                _tileToToken.Remove(_tokenToTile[token]);
                _tokenToTile.Remove(token);
            }

            PlaceTokenImpl(token, tile);
        }

        public void SwapTokens(TToken tokenA, TToken tokenB)
        {
            Assert.IsTrue(_tokenToTile.ContainsKey(tokenA), $"{name} {tokenA} is not on board.");
            Assert.IsTrue(_tokenToTile.ContainsKey(tokenB), $"{name} {tokenB} is not on board.");
            var tileA = _tokenToTile[tokenA];
            var tileB = _tokenToTile[tokenB];
            _tileToToken.Remove(tileA);
            _tileToToken.Remove(tileB);
            _tokenToTile.Remove(tokenA);
            _tokenToTile.Remove(tokenB);
            PlaceTokenImpl(tokenA, tileB);
            PlaceTokenImpl(tokenB, tileA);
        }

        public void RemoveToken(TToken token)
        {
            Assert.IsTrue(_tokenToTile.ContainsKey(token), $"{name} {token} is not on board.");
            JTile tile = _tokenToTile[token];
            RemoveTokenImpl(tile, token);
        }

        public void FreeTile(JTile tile)
        {
            Assert.IsTrue(_tileToToken.ContainsKey(tile), $"{name} {tile} is not tracked.");
            TToken token = _tileToToken[tile];
            RemoveTokenImpl(tile, token);
        }

        public void ResetThis()
        {
            _tokenToTile.Clear();
            _tileToToken.Clear();
            _allTokens.Clear();
        }

        // --------------- IMPLEMENTATION --------------- //
        private void PlaceTokenImpl(TToken token, JTile tile)
        {
            Assert.IsTrue(IsTileFree(tile), $"{name} - {tile} contains {GetTokenOnTile(tile)}. Cannot place {token}");
            _tileToToken[tile]  = token;
            _tokenToTile[token] = tile;
            if (!_allTokens.Contains(token)) { _allTokens.Add(token); }

            OnTokenPlaced?.Invoke((token, tile));
        }
        
        private void RemoveTokenImpl(JTile tile, TToken token)
        {
            _tileToToken.Remove(tile);
            _tokenToTile.Remove(token);
            _allTokens.Remove(token);
            OnTokenRemoved?.Invoke((token, tile));
        }
    }
}
