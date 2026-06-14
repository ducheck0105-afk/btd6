# BTD6 — Bảng Upgrade Cost + Stats

> **⚠️ Accuracy warning:** Số liệu viết từ memory — các giá trị đánh dấu ⚠️ cần verify lại từ wiki.  
> Cost tính theo **Medium difficulty**. Easy = ×0.85 · Hard = ×1.08 · Impoppable = ×1.2

---

## Hệ thống 3 loại nâng cấp

```
Tower upgrade flow:
  Tower XP (kill bloon) → Unlock upgrade permanently
  In-game Gold          → Mua upgrade đã unlock trong ván
  MK Points (level up)  → Passive buff permanent, không liên quan upgrade
```

---

### Loại 1 — Tower XP Unlock (mở khóa upgrade vĩnh viễn)

- Mỗi tower type có **pool XP riêng** — Dart Monkey XP, Ninja XP, v.v.
- XP tích lũy khi **chính tower đó** tiêu diệt bloon trong bất kỳ ván nào
- Dùng XP để **unlock từng upgrade** — một lần duy nhất, áp dụng mọi ván sau
- Upgrade chưa unlock = **greyed out**, không thể mua dù có đủ gold
- Mỗi tier tốn XP nhiều hơn tier trước (xem bảng từng tower bên dưới)
- Có nút **"Unlock All"** — mua tất cả upgrade của 1 tower bằng tiền thật (IAP)

> Ví dụ từ screenshot: Dart Monkey T4 Juggernaut cần **7,500 XP** để unlock, sau đó mua bằng **1,530 gold** trong ván.

---

### Loại 2 — In-game Gold (mua upgrade đã unlock trong ván)

- Mỗi tower có **3 path**, mỗi path **5 tier**
- Mua bằng gold trong ván chơi — chỉ mua được upgrade đã unlock
- **Giới hạn cross-path:**
  - Tối đa 1 path lên **Tier 5**, 1 path khác lên **Tier 2**, path còn lại = 0
  - Hoặc 2 path lên **Tier 3** (không path nào lên T4+)
  - Ký hiệu: `5-2-0`, `0-5-2`, `2-0-5`, `3-2-0`, `0-2-3`...

---

### Loại 3 — Monkey Knowledge (MK) — passive buff meta

- Mua bằng **MK Points** (kiếm bằng lên level player account — mỗi level = 1 MKP)
- Buff **passive permanent** áp dụng mọi ván — **không phải unlock upgrade**, chỉ tăng chỉ số base
- Chia nhóm: Primary MK · Military MK · Magic MK · Support MK · Hero MK · General MK
- Mỗi node trong cây MK có giá **1–3 MKP**
- Node phải mở theo thứ tự: cần mở node cha trước mới mở được node con

> Chi tiết đầy đủ xem section **"Monkey Knowledge Tree"** bên dưới.

---

## Chỉ số base (Stats)

> Damage = số RBE xóa mỗi phát | Pierce = số bloon 1 đạn hit được | Range = đơn vị game

| Tower | Damage | Pierce | Range | Cooldown | Ghi chú |
|---|---|---|---|---|---|
| Dart Monkey | 1 | 1 | 32 | 0.95s | |
| Boomerang Monkey | 1 | 5 | 43 | 1.0s | Boomerang cong, hit nhiều |
| Bomb Shooter | 1 + AoE 1 | 40 (AoE) | 46 | 1.5s | Bom nổ radius ~18 |
| Tack Shooter | 1 | 1 | 20 | 1.0s | 8 hướng/lần |
| Ice Monkey | 0 (freeze) | ∞ | 25 | 2.0s | Freeze không damage |
| Glue Gunner | 0 (slow) | 1 | 37 | 1.5s | |
| Desperado | 1 | 2 | 40 | 0.7s ⚠️ | 2 súng xen kẽ |
| Sniper Monkey | 2 | 1 | ∞ | 1.7s | Xuyên 1 layer lead (base) |
| Monkey Sub | 1 | 4 | 42 | 0.65s | |
| Monkey Buccaneer | 1 dart / 3 cannon | 2 / 1 | 45 | 1.0s ⚠️ | |
| Monkey Ace | 1 | 2 | map | 1.0s ⚠️ | Phụ thuộc pattern bay |
| Heli Pilot | 1 | 2 | 60 | 0.35s | Di chuyển được |
| Mortar Monkey | 3 (AoE) | 40 | ∞ | 1.5s | |
| Dartling Gunner | 1 | 2 | 100 | 0.1s/burst ⚠️ | Follow chuột |
| Wizard Monkey | 2 | 1 | 45 | 0.7s | |
| Super Monkey | 1 | 1 | 45 | 0.06s | Bắn cực nhanh |
| Ninja Monkey | 1 | 2 | 40 | 0.5s | Camo tự nhiên |
| Alchemist | 1 (acid) | AoE | 45 | 2.5s | Buff = không có "damage" |
| Druid | 1 | 3 | 40 | 0.9s | |
| Mermonkey | 1 | 3 | 40 | 0.8s ⚠️ | |
| Banana Farm | 0 | — | — | per round | Tạo tiền |
| Spike Factory | 1 | 2 | 40 | 1.0s | Đặt spike tích lũy |
| Monkey Village | 0 | — | 40 | — | Buff aura |
| Engineer Monkey | 1 | 2 | 45 | 0.6s | + sentry gun |
| Beast Handler | varies | — | — | — ⚠️ | Phụ thuộc beast loại nào |

---

## PRIMARY — Upgrade Costs

### Dart Monkey ($200)

> Tên upgrade xác nhận từ screenshot in-game.

| Tier | Path 1 — Juggernaut | Path 2 — Quick Shots | Path 3 — Crossbow |
|---|---|---|---|
| T1 | Sharp Shots | Quick Shots | Long Range Darts |
| T2 | Razor Sharp Shots | Very Quick Shots | Enhanced Eyesight |
| T3 | Spike-o-pult | Triple Shot | Crossbow |
| T4 | Juggernaut | Super Monkey Fan Club | Sharp Shooter |
| T5 | Ultra-Juggernaut | Plasma Monkey Fan Club | Crossbow Master |

**Gold cost (in-game, Medium):**

| Tier | Path 1 | Path 2 | Path 3 |
|---|---|---|---|
| T1 | $140 | $140 | $140 |
| T2 | $170 | $250 ⚠️ | $200 |
| T3 | $400 | $500 ⚠️ | $500 ⚠️ |
| T4 | **$1,530** ✓screenshot | $3,200 ⚠️ | $2,800 ⚠️ |
| T5 | $15,000 ⚠️ | $20,000 ⚠️ | $18,000 ⚠️ |

**XP Unlock cost (Tower XP):**

| Tier | XP cần | Ghi chú |
|---|---|---|
| T1 | ~200 XP ⚠️ | |
| T2 | ~500 XP ⚠️ | |
| T3 | ~1,500 XP ⚠️ | |
| T4 | **7,500 XP** ✓screenshot | Juggernaut |
| T5 | ~35,000 XP ⚠️ | |

*Path 1: pierce tăng dần → T3 bóng gai (pierce 5) → T4 (pierce 50) → T5 (tách 8 mảnh)*  
*Path 2: tốc độ tăng → T4/T5 là ability triệu hồi Super Monkey tạm thời*  
*Path 3: range + damage → T5 Crossbow Master tốc độ rất cao, damage cao*

---

### Boomerang Monkey ($325)
| Tier | Path 1 — Bionic | Path 2 — Glaive | Path 3 — Sai |
|---|---|---|---|
| T1 | Faster Throwing — **$175** | Glaive Ricochet — **$200** | Bionic Boomerang — **$200** ⚠️ |
| T2 | Faster Throwing 2 — **$350** ⚠️ | Glaive Lord — **$350** ⚠️ | Turbo Charge — **$350** ⚠️ |
| T3 | Bionic Boomerang — **$850** ⚠️ | Glaive Riccochet — **$1,200** ⚠️ | Perma-Charge — **$1,200** ⚠️ |
| T4 | MOAB Domination — **$3,800** ⚠️ | Glaive Lord — **$3,800** ⚠️ | MOAB Press — **$3,500** ⚠️ |
| T5 | Permanent Charge — **$22,500** ⚠️ | Glaive Lord Ultimate — **$22,500** ⚠️ | Sai Special Forces — **$25,000** ⚠️ |

> ⚠️ Nhiều số trong bảng Boomerang cần verify — tao không tự tin với tier names + costs.

---

### Bomb Shooter ($525)
| Tier | Path 1 — Bigger Bombs | Path 2 — Faster Bombs | Path 3 — Extra |
|---|---|---|---|
| T1 | Bigger Bombs — **$250** | Faster Fuse — **$250** | Extra Range — **$175** |
| T2 | Bloon Impact — **$350** ⚠️ | Missile Launcher — **$400** ⚠️ | Even Bigger Bombs — **$250** ⚠️ |
| T3 | MOAB Mauler — **$900** | Cluster Bombs — **$850** ⚠️ | Bloon Impact — **$750** ⚠️ |
| T4 | MOAB Assassin — **$6,500** ⚠️ | Bomb Blitz — **$4,200** ⚠️ | MOAB Eliminator — **$4,200** ⚠️ |
| T5 | MOAB Eliminator — **$22,000** ⚠️ | Bomb Blitz T5 — **$22,000** ⚠️ | Bloon Crush — **$19,000** ⚠️ |

---

### Tack Shooter ($200)
| Tier | Path 1 — Maelstrom | Path 2 — Inferno Ring | Path 3 — Super Maelstrom |
|---|---|---|---|
| T1 | More Tacks — **$150** | Faster Shooting — **$150** | Long Range Tacks — **$100** |
| T2 | Even More Tacks — **$200** | Even Faster — **$200** | Super Range — **$200** ⚠️ |
| T3 | Tack Sprayer — **$500** | Hot Shots — **$450** | Blade Shooter — **$300** ⚠️ |
| T4 | Blade Maelstrom — **$3,000** | Ring of Fire — **$3,500** | Tack Zone — **$2,000** ⚠️ |
| T5 | Super Maelstrom — **$16,000** ⚠️ | Inferno Ring — **$18,000** ⚠️ | — **$22,000** ⚠️ |

---

### Ice Monkey ($500)
| Tier | Path 1 — Brittle | Path 2 — Arctic | Path 3 — Icicle |
|---|---|---|---|
| T1 | Permafrost — **$200** | Enhanced Freeze — **$200** | Cold Snap — **$250** ⚠️ |
| T2 | Cold Snap — **$250** ⚠️ | Metal Freeze — **$350** | Embrittlement — **$350** ⚠️ |
| T3 | Ice Shards — **$1,200** ⚠️ | Arctic Wind — **$1,400** | Cryo Cannon — **$2,200** ⚠️ |
| T4 | Snowstorm — **$2,800** ⚠️ | Snowstorm — **$2,800** ⚠️ | Icicle Impale — **$5,000** ⚠️ |
| T5 | Absolute Zero — **$22,000** ⚠️ | Super Brittle — **$24,000** ⚠️ | — **$18,000** ⚠️ |

---

### Glue Gunner ($275)
| Tier | Path 1 — Bloon Solver | Path 2 — Glue Storm | Path 3 — MOAB Glue |
|---|---|---|---|
| T1 | Glue Soak — **$100** | Stickier Glue — **$100** | Bigger Globs — **$100** |
| T2 | Corrosive Glue — **$200** | Yellower Glue — **$200** ⚠️ | Glue Splatter — **$300** ⚠️ |
| T3 | Bloon Dissolver — **$1,200** ⚠️ | Glue Hose — **$800** ⚠️ | Glue Striker — **$400** ⚠️ |
| T4 | Bloon Liquefier — **$3,500** ⚠️ | Glue Strike — **$2,500** ⚠️ | MOAB Glue — **$2,200** ⚠️ |
| T5 | Bloon Solver — **$10,000** ⚠️ | Glue Storm — **$12,000** ⚠️ | Super Glue — **$16,000** ⚠️ |

---

## MILITARY — Upgrade Costs

### Sniper Monkey ($350)
| Tier | Path 1 — MOAB | Path 2 — Supply Drop | Path 3 — Elite |
|---|---|---|---|
| T1 | Full Metal Jacket — **$250** | Night Vision Goggles — **$300** | Faster Firing — **$200** |
| T2 | Large Calibre — **$400** | Shrapnel Shot — **$350** ⚠️ | Even Faster — **$250** ⚠️ |
| T3 | Deadly Precision — **$1,800** ⚠️ | Bouncing Bullet — **$1,800** ⚠️ | Semi-Automatic — **$1,200** ⚠️ |
| T4 | Maim MOAB — **$2,500** ⚠️ | Supply Drop — **$3,000** | Full Auto Sniper — **$2,500** ⚠️ |
| T5 | Cripple MOAB — **$17,500** ⚠️ | Elite Sniper — **$12,500** ⚠️ | Elite Defender — **$9,500** ⚠️ |

---

### Monkey Sub ($325)
| Tier | Path 1 — Nuke | Path 2 — Advanced Intel | Path 3 — Pre-Emptive |
|---|---|---|---|
| T1 | Longer Range — **$100** | Airburst Darts — **$400** ⚠️ | Barbed Darts — **$150** ⚠️ |
| T2 | Advanced Range — **$250** ⚠️ | Advanced Intel — **$800** ⚠️ | Heat-tipped Darts — **$400** ⚠️ |
| T3 | Sub Commander — **$800** ⚠️ | Submerge & Support — **$700** ⚠️ | Ballistic Missile — **$2,000** ⚠️ |
| T4 | First Strike Capability — **$12,500** | Bloontonium Lab — **$2,000** ⚠️ | Pre-Emptive Strike — **$6,500** ⚠️ |
| T5 | Carrier Flagship — **$26,000** ⚠️ | Energizer — **$26,000** ⚠️ | Sub Commander — **$18,000** ⚠️ |

---

### Heli Pilot ($1,600)
| Tier | Path 1 — Apache | Path 2 — Support Chinook | Path 3 — Special Ops |
|---|---|---|---|
| T1 | Bigger Jets — **$350** ⚠️ | Faster Darts — **$350** ⚠️ | IFR — **$500** ⚠️ |
| T2 | IFR — **$500** ⚠️ | Faster Firing — **$400** ⚠️ | Lots More Darts — **$600** ⚠️ |
| T3 | Quad Darts — **$1,200** ⚠️ | MOAB Shove — **$2,000** ⚠️ | Downdraft — **$1,800** ⚠️ |
| T4 | Apache Dartship — **$8,000** ⚠️ | Support Chinook — **$5,600** ⚠️ | Special Poperations — **$5,000** ⚠️ |
| T5 | Apache Prime — **$24,000** ⚠️ | Flying Fortress — **$25,000** ⚠️ | Commanche Commander — **$24,000** ⚠️ |

---

### Mortar Monkey ($750)
| Tier | Path 1 — The Biggest One | Path 2 — Pop and Awe | Path 3 — Splatmaster |
|---|---|---|---|
| T1 | Bigger Blast — **$350** ⚠️ | Faster Reload — **$300** ⚠️ | Increased Accuracy — **$150** |
| T2 | Bloon Buster — **$500** ⚠️ | Rapid Reload — **$450** ⚠️ | Burny Stuff — **$250** ⚠️ |
| T3 | Shell Shock — **$1,200** ⚠️ | Heavy Shells — **$700** ⚠️ | Shattering Shells — **$1,400** ⚠️ |
| T4 | The Big One — **$4,000** ⚠️ | Artillery Battery — **$4,500** ⚠️ | The Bloon Buster — **$3,200** ⚠️ |
| T5 | The Biggest One — **$23,000** ⚠️ | Pop and Awe — **$21,500** ⚠️ | Splatmaster — **$20,000** ⚠️ |

---

### Dartling Gunner ($1,500)
| Tier | Path 1 — Ray of Doom | Path 2 — MAD | Path 3 — Bloon Exclusion Zone |
|---|---|---|---|
| T1 | Focused Firing — **$500** ⚠️ | Faster Barrel Spin — **$400** ⚠️ | Longer Range — **$250** ⚠️ |
| T2 | Laser Shock — **$800** ⚠️ | Faster Swivel — **$550** ⚠️ | Even Longer Range — **$400** ⚠️ |
| T3 | Laser Cannon — **$3,400** ⚠️ | Hydra Rocket Pods — **$4,100** ⚠️ | Dartling Cannon — **$3,200** ⚠️ |
| T4 | Plasma Accelerator — **$12,000** ⚠️ | M.A.D — **$18,000** | Buckshot — **$6,500** ⚠️ |
| T5 | Ray of Doom — **$60,000** ⚠️ | Bloon Exclusion Zone — **$45,000** ⚠️ | Centurion — **$25,000** ⚠️ |

---

## MAGIC — Upgrade Costs

### Ninja Monkey ($500)
| Tier | Path 1 — Master Bomber | Path 2 — Grand Saboteur | Path 3 — Grandmaster Ninja |
|---|---|---|---|
| T1 | Ninja Discipline — **$200** | Distraction — **$200** | Double Shot — **$250** |
| T2 | Sharp Shurikens — **$300** | Counter-Espionage — **$400** | Seeking Shuriken — **$300** ⚠️ |
| T3 | Bloonjitsu — **$900** | Shinobi Tactics — **$600** | Caltrops — **$500** ⚠️ |
| T4 | Master Bomber — **$3,000** ⚠️ | Bloon Sabotage — **$4,200** ⚠️ | Four Arrows — **$3,500** ⚠️ |
| T5 | Grandmaster Ninja — **$18,500** ⚠️ | Grand Saboteur — **$25,500** ⚠️ | Master Bomber — **$26,500** ⚠️ |

---

### Alchemist ($550)
| Tier | Path 1 — Permanent Brew | Path 2 — Rubber to Gold | Path 3 — Transforming Tonic |
|---|---|---|---|
| T1 | Larger Potions — **$200** ⚠️ | Stronger Acid — **$200** ⚠️ | Faster Throwing — **$150** ⚠️ |
| T2 | Acidic Mixture Dip — **$350** ⚠️ | Perishing Potions — **$300** ⚠️ | More Potions — **$250** ⚠️ |
| T3 | Berserker Brew — **$1,400** | Unstable Concoction — **$1,400** ⚠️ | Stronger Stimulant — **$900** ⚠️ |
| T4 | Stronger Stimulant — **$1,600** ⚠️ | Rubber to Gold — **$3,000** ⚠️ | Super Simian Stuff — **$4,000** ⚠️ |
| T5 | Permanent Brew — **$18,000** ⚠️ | Bloon Master Alchemist — **$40,000** ⚠️ | Transforming Tonic — **$16,000** ⚠️ |

---

### Super Monkey ($2,500)
| Tier | Path 1 — True Sun God | Path 2 — Dark Champion | Path 3 — Robo Monkey |
|---|---|---|---|
| T1 | Laser Blasts — **$2,500** ⚠️ | Super Range — **$1,000** ⚠️ | Robo Monkey — **$2,000** ⚠️ |
| T2 | Plasma Blasts — **$3,500** ⚠️ | Epic Range — **$1,500** ⚠️ | Laser Blasts P2 — **$3,500** ⚠️ |
| T3 | Sun Avatar — **$20,000** ⚠️ | Dark Knight — **$12,000** ⚠️ | Robo Tech — **$12,000** ⚠️ |
| T4 | Sun Temple — **$100,000** | Dark Champion — **$75,000** ⚠️ | Tech Terror — **$50,000** ⚠️ |
| T5 | True Sun God — **$500,000** | Legend of the Night — **$200,000** ⚠️ | The Anti-Bloon — **$200,000** ⚠️ |

> Super Monkey tốn kém nhất game. T5 True Sun God yêu cầu sacrifice towers.

---

## SUPPORT — Upgrade Costs

### Banana Farm ($1,250)
| Tier | Path 1 — Banana Central | Path 2 — Monkey Wall Street | Path 3 — Marketplace |
|---|---|---|---|
| T1 | More Bananas — **$400** | Banana Republic — **$600** ⚠️ | Banana Salvage — **$200** |
| T2 | Banana Plantation — **$900** | Backroom Deals — **$800** ⚠️ | Marketplace — **$1,200** |
| T3 | Banana Research Facility — **$3,000** | Offshore Accounts — **$3,500** ⚠️ | Central Market — **$2,500** |
| T4 | Banana Central — **$19,000** | Monkey Bank — **$7,000** ⚠️ | IMF Loan — **$7,500** ⚠️ |
| T5 | Banana HQ — **$50,000** ⚠️ | Monkey Wall Street — **$60,000** ⚠️ | Monkey-Nomics — **$42,000** ⚠️ |

*T4 Path 2 "Monkey Bank" tích lũy tiền qua vòng, rút tay → lãi cao. T4 Path 3 "IMF Loan" cho vay $10,000 trả dần.*

---

### Monkey Village ($1,200)
| Tier | Path 1 — Primary Mentoring | Path 2 — Monkeyopolis | Path 3 — High Energy |
|---|---|---|---|
| T1 | Bigger Radius — **$600** | Jungle Drums — **$900** | Monkey Business — **$500** |
| T2 | Jungle Drums — **$900** ⚠️ | Radar Scanner — **$1,200** ⚠️ | Monkey Commerce — **$800** ⚠️ |
| T3 | Primary Training — **$1,800** ⚠️ | Jungle's Bounty — **$1,800** ⚠️ | Monkey Town — **$3,000** ⚠️ |
| T4 | Primary Mentoring — **$5,000** ⚠️ | Monkeyopolis — **$8,000** ⚠️ | High Energy Beacon — **$4,500** ⚠️ |
| T5 | Primary Expertise — **$20,000** ⚠️ | Monkeyopolis T5 — **$30,000** ⚠️ | Homeland Defense — **$25,000** ⚠️ |

---

### Engineer Monkey ($400)
| Tier | Path 1 — XXXL Trap | Path 2 — Ultraboost | Path 3 — Cleansing Foam |
|---|---|---|---|
| T1 | Sentry Expert — **$350** ⚠️ | Faster Engineering — **$250** ⚠️ | Nail Gun — **$200** ⚠️ |
| T2 | Sentry Champion — **$500** ⚠️ | Sprockets — **$350** ⚠️ | Faster Shooting — **$300** ⚠️ |
| T3 | Sentry Paragon — **$1,000** ⚠️ | Overclock — **$2,400** | Compressed-Air Blasters — **$800** ⚠️ |
| T4 | XXXL Trap — **$6,500** ⚠️ | Ultraboost — **$12,500** ⚠️ | Cleansing Foam — **$3,500** ⚠️ |
| T5 | Apex Plasma Master — **$30,000** ⚠️ | Sentry Champion — **$35,000** ⚠️ | Viral Foam — **$28,000** ⚠️ |

---

---

## Monkey Knowledge Tree

> MK Points kiếm bằng cách lên level player account (1 level = 1 MKP).  
> Mỗi node cần mở node cha trước. Cost phổ biến nhất = **1 MKP**.  
> ⚠️ Một số tên node và cost cần verify.

### General MK (áp dụng toàn bộ game)

| Node | Cost | Effect |
|---|---|---|
| Monkey Education | 1 MKP | Tất cả tower nhận XP nhanh hơn (dùng cho leveling hero) |
| Monkey Business | 1 MKP | Tower rẻ hơn 1% ⚠️ |
| Monkey Dividends | 1 MKP | Bán tower hoàn lại % cao hơn |
| Healthy Bananas | 1 MKP | Bắt đầu ván với thêm mạng |
| Monkey City | 1 MKP | Thêm tiền bắt đầu ván |
| Better Sell Deals | 1 MKP | Bán tower 80% thay vì 70% ⚠️ |
| Brickell's Tune | 1 MKP | Hero lên level nhanh hơn |
| More Camo Detection | 1 MKP | Một số tower base có tầm camo detect xa hơn ⚠️ |

---

### Primary MK

| Node | Cost | Tower | Effect |
|---|---|---|---|
| Primary Training | 1 MKP | Dart Monkey | +1 pierce base |
| Bigger Darts | 1 MKP | Dart Monkey | +range |
| Cheap Primary | 1 MKP | Tất cả Primary | Giảm giá 1 số upgrade ⚠️ |
| More Tacks | 1 MKP | Tack Shooter | +pierce base |
| Tack Shooter Buff | 1 MKP | Tack Shooter | Faster base attack ⚠️ |
| Splodey Darts | 1 MKP | Dart Monkey | Đôi khi đạn nổ nhỏ |
| Glue Splatter | 1 MKP | Glue Gunner | AoE keo nhỏ từ base |
| Bigger Ice | 1 MKP | Ice Monkey | +radius freeze |
| Boomerangs on 3 | 1 MKP | Boomerang Monkey | Base bắn 3 thay 1 ⚠️ |
| Primary Expertise | 2 MKP | Tất cả Primary | Damage tăng nhẹ tất cả Primary ⚠️ |
| Bomb Range | 1 MKP | Bomb Shooter | +range |
| Cheaper Primary Upgrades | 2 MKP | Tất cả Primary | Giảm giá upgrade 5% ⚠️ |

---

### Military MK

| Node | Cost | Tower | Effect |
|---|---|---|---|
| Rapid Fire | 1 MKP | Sniper Monkey | +attack speed |
| Cheap Shot | 1 MKP | Sniper Monkey | Giảm giá upgrade |
| Long Shot | 1 MKP | Sniper Monkey | +range (range đã là ∞, ảnh hưởng đạn đặc biệt) ⚠️ |
| Lots More Darts | 1 MKP | Monkey Sub | +pierce |
| Deeper Dives | 1 MKP | Monkey Sub | +range dưới nước |
| Guided Magic | 1 MKP | Monkey Ace | Đạn tự tìm mục tiêu tốt hơn ⚠️ |
| Fighter Plane | 1 MKP | Monkey Ace | Bay nhanh hơn ⚠️ |
| Pilot License | 1 MKP | Heli Pilot | +attack speed |
| Mortar Calibration | 1 MKP | Mortar Monkey | +accuracy (vùng nổ nhỏ hơn) |
| Big Cannon | 1 MKP | Bomb Shooter | +damage ⚠️ |
| Military Precision | 2 MKP | Tất cả Military | Tăng damage nhẹ ⚠️ |
| Fast Upgrades | 1 MKP | Heli Pilot | Giảm giá upgrade ⚠️ |
| Buccaneer's Edge | 1 MKP | Monkey Buccaneer | +pierce dart |

---

### Magic MK

| Node | Cost | Tower | Effect |
|---|---|---|---|
| Intense Magic | 1 MKP | Wizard Monkey | +damage phép |
| Extra Dart | 1 MKP | Ninja Monkey | Base bắn thêm 1 shuriken ⚠️ |
| Shinobi Tactics | 1 MKP | Ninja Monkey | Ninja gần nhau buff nhau (sớm hơn base) ⚠️ |
| Alchemist Knowledge | 1 MKP | Alchemist | Brew buff kéo dài hơn |
| Druid's Roar | 1 MKP | Druid | +pierce sét |
| Nature's Ward | 1 MKP | Druid | +range |
| Super Shots | 1 MKP | Super Monkey | +attack speed nhẹ |
| Super Range | 1 MKP | Super Monkey | +range |
| Flat Camo Detection | 1 MKP | Ninja / Druid | Detect camo xa hơn |
| Arcane Mastery | 2 MKP | Tất cả Magic | +damage tất cả Magic ⚠️ |
| Mermonkey Knowledge | 1 MKP | Mermonkey | +pierce ⚠️ |

---

### Support MK

| Node | Cost | Tower | Effect |
|---|---|---|---|
| More Bananas | 1 MKP | Banana Farm | +tiền mỗi round |
| Banana Investments | 1 MKP | Banana Farm | Bank tích lũy nhiều hơn |
| Prickly Spikes | 1 MKP | Spike Factory | Spike tồn tại lâu hơn |
| Bigger Village | 1 MKP | Monkey Village | +radius aura |
| Flat Packed | 1 MKP | Monkey Village | Giảm giá mua |
| Better Camo Detection | 1 MKP | Monkey Village | Radius camo detect lớn hơn ⚠️ |
| Sentry Expertise | 1 MKP | Engineer Monkey | Sentry mạnh hơn |
| Overclock Power | 1 MKP | Engineer Monkey | Overclock hiệu quả hơn ⚠️ |
| Bigger Coalitions | 1 MKP | Monkey Village | +10% range buff ⚠️ |
| Support Mastery | 2 MKP | Tất cả Support | Giảm giá upgrade Support 5% ⚠️ |
| Beast Knowledge | 1 MKP | Beast Handler | Beast merge nhanh hơn ⚠️ |

---

### Hero MK

| Node | Cost | Effect |
|---|---|---|
| Hero Favors | 1 MKP | Tất cả hero rẻ hơn 10% |
| Hero Platoon | 1 MKP | Hero nhận XP nhanh hơn 10% |
| Double XP | 2 MKP | XP hero × 2 trong 1 round đầu ⚠️ |
| Veteran Monkey Training | 2 MKP | Hero bắt đầu ở level 3 thay vì 1 ⚠️ |
| Hero Boost | 1 MKP | Hero lên level nhanh hơn khi gần cluster tower ⚠️ |

---

## Ghi chú thiết kế (cho game clone)

Dựa vào cấu trúc BTD6, game clone nên có:

| Yếu tố | Gợi ý |
|---|---|
| **Số path** | 3 path giống BTD6, hoặc đơn giản hóa thành 2 path |
| **Số tier** | 3 tier (không cần đến 5) cho phiên bản đơn giản hóa |
| **Cross-path limit** | Max 1 path T3 + 1 path T2 nếu dùng 3 tier |
| **Cost scaling** | Mỗi tier gấp ~2–3x tier trước. T3 thường rất đắt |
| **Monkey Knowledge** | Có thể thay bằng "Passive Skill Tree" mua bằng XP meta |
| **Base stats** | Damage thấp (1–2), pierce thấp (1–5) — upgrade mới tăng nhiều |

---

*⚠️ Nhiều con số trong file này cần verify từ wiki chính thức trước khi dùng làm tham khảo thiết kế chính xác.*
