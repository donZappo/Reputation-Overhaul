# Reputation-Overhaul

## Goals

- Eliminate or ameliorate the gamey and grindy nature of the current reputation system gating contracts based on faction specific reputation level to .5 skulls.
- Change the modelling from a straight friendship model to something that models faction specific working relationship (reputation) status and trust, as well as overall galactic prestige (MRB).
- Introduce meaningful rewards for increasing either reputation or MRB rating.
- Introduce meaningful choices for increasing either reputation or MRB rating.
- Introduce bonuses to reputation/MRB for dropping difficulty appropriate lances.
- Reputation is a model of the working relationship between the company and a faction.
- MRB rating is a model of the company's prestige and fame. 


## Effects of Reputation

- Rep Tiers: Loathed/Hated/Disliked/Indifferent/Liked/Friendly/Honored
- No longer affects contract payout.
- Shops inaccessible if Hated (in addition to Loathed).
- Shop price adjustment per level: (N/A, N/A, 0.15, 0, -0.05, -0.1, -0.15)
  - Original adjustment values: (N/A, 0.5, 0.2, 0.1, 0, -0.05, -0.1)
- Max Contract Availability Per Tier: (1, 2, 3, 5, 7, 8, 9)
  - Original: (1, 2, 3, 6, 8, 9, 10)
- Affects max contract difficulty in concert with MRB rating.
- Determines availability of alliance - changed to 75 reputation.
- Determines break alliance penalty - unchanged.
- Impact on Rep Values (original value):
  - TargetRepSuccessMod: -0.22 (-0.8)
  - TargetRepGoodFaithMod: -0.12 (-0.4)
  - TargetRepBadFaithMod: 0 (0)
  - EmployerRepSuccessMod: 1.1 (1)
  - EmployerRepGoodFaithMod: 0.6 (0.5)
  - EmployerRepBadFaithMod: -2 (-2)
 
 
## Effects of MRB Rating

- Determines advanced Mechwarrior hire availability - unchanged.
- MRB Rating increases contract pay. Pay = Pay + Pay * (MRB / MRB_Max)
  - MRB_Max = 1000 as set in SimGameConstants File
- Rating expands contract availability range.
  - Follows vanilla behavior (with faction rep) but adds in the MRB tier of the player.
  - Tiers go from 1-5. 
- Contracts per system raised by MRB Rating.

