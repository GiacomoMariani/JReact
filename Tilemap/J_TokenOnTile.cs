using System.Collections.Generic;
using JReact.Tilemaps;
using Sirenix.OdinInspector;
using UnityEngine;
using UnityEngine.Assertions;

namespace System
{
    public abstract class J_TokenOnTile<TToken, TTile> : ScriptableObject
        where TTile : J_Tile
    {
        // --------------- EVENTS --------------- //
        public Action<(TToken token, TTile tile)> OnTokenPlaced;
        public Action<(TToken token, TTile tile)> OnTokenRemoved;

        // --------------- FIELDS AND PROPERTIES --------------- //
        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected Dictionary<TToken, TTile> _tokenToTile = new Dictionary<TToken, TTile>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector]
        protected Dictionary<TTile, TToken> _tileToToken = new Dictionary<TTile, TToken>();

        [FoldoutGroup("State", false, 5), ReadOnly, ShowInInspector] protected List<TToken> _allTokens = new List<TToken>(50);

        // --------------- QUERIES --------------- //
        public TToken GetTokenOnTile(TTile tile) => !_tileToToken.ContainsKey(tile)
                                                        ? default
                                                        : _tileToToken[tile];

        public TTile GetTileFromToken(TToken token) => !_tokenToTile.ContainsKey(token)
                                                           ? default
                                                           : _tokenToTile[token];

        public virtual bool IsTileFree(TTile tile)
        {
            if (!_tileToToken.ContainsKey(tile)) return true;
            else return _tileToToken[tile] == null;
        }

        public bool IsTokenOnBoard(TToken token) => _tokenToTile.ContainsKey(token);

        public TToken GetToken(int index) => _allTokens[index];

        // --------------- COMMANDS --------------- //
        public void PlaceTokenOnTile(TToken token, TTile tile)
        {
            Assert.IsTrue(IsTileFree(tile), $"{name} - {tile} contains {GetTokenOnTile(tile)}. Cannot place {token}");
            if (_tokenToTile.ContainsKey(token))
            {
                _tileToToken.Remove(_tokenToTile[token]);
                _tokenToTile.Remove(token);
            }

            SetTokenOnTile(token, tile);
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
            SetTokenOnTile(tokenA, tileB);
            SetTokenOnTile(tokenB, tileA);
        }

        public void RemoveToken(TToken token)
        {
            var tile = _tokenToTile[token];
            Assert.IsTrue(_tokenToTile.ContainsKey(token), $"{name} {token} is not on board.");
            _tileToToken.Remove(tile);
            _tokenToTile.Remove(token);
            _allTokens.Remove(token);
            OnTokenRemoved?.Invoke((token, tile));
        }

        public void FreeTile(TTile tile)
        {
            var token = _tileToToken[tile];
            Assert.IsTrue(_tileToToken.ContainsKey(tile), $"{name} {tile} is not tracked.");
            _tokenToTile.Remove(token);
            _tileToToken.Remove(tile);
            _allTokens.Remove(token);
            OnTokenRemoved?.Invoke((token, tile));
        }

        public void ResetThis()
        {
            _tokenToTile.Clear();
            _tileToToken.Clear();
            _allTokens.Clear();
        }

        private void SetTokenOnTile(TToken token, TTile tile)
        {
            Assert.IsTrue(IsTileFree(tile), $"{name} - {tile} contains {GetTokenOnTile(tile)}. Cannot place {token}");
            _tileToToken[tile]  = token;
            _tokenToTile[token] = tile;
            if (!_allTokens.Contains(token)) _allTokens.Add(token);
            OnTokenPlaced?.Invoke((token, tile));
        }
    }
}
