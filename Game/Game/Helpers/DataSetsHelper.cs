﻿using System;
using System.Threading.Tasks;

using Game.ViewModels;

namespace Game.Helpers
{
    static public class DataSetsHelper
    {
        static public bool WarmUp()
        {
            _ = ScoreIndexViewModel.Instance.GetCurrentDataSource();
            _ = ItemIndexViewModel.Instance.GetCurrentDataSource();
            _ = CharacterIndexViewModel.Instance.GetCurrentDataSource();

            return true;
        }

        private static readonly object WipeLock = new object();

        /// <summary>
        /// Call the Wipe routines in order one by one
        /// </summary>
        /// <returns></returns>
        static public async Task<bool> WipeDataInSequence()
        {
            lock (WipeLock)
            {
                var runSyncScore = Task.Factory.StartNew(new Func<Task>(async () =>
                {
                    _ = await ScoreIndexViewModel.Instance.DataStoreWipeDataListAsync();
                    await Task.Delay(100);
                })).Unwrap();
                runSyncScore.Wait();


                var runSyncItem = Task.Factory.StartNew(new Func<Task>(async () =>
                {
                    _ = await ItemIndexViewModel.Instance.DataStoreWipeDataListAsync();
                    await Task.Delay(100);
                })).Unwrap();
                runSyncItem.Wait();

                var runSyncCharacter = Task.Factory.StartNew(new Func<Task>(async () =>
                {
                    _ = await CharacterIndexViewModel.Instance.DataStoreWipeDataListAsync();
                    await Task.Delay(100);
                })).Unwrap();
                runSyncCharacter.Wait();

                var runSyncMonster = Task.Factory.StartNew(new Func<Task>(async () =>
                {
                    _ = await MonsterIndexViewModel.Instance.DataStoreWipeDataListAsync();
                    await Task.Delay(100);
                })).Unwrap();
                runSyncMonster.Wait();
            }

            return await Task.FromResult(true);
        }
    }
}