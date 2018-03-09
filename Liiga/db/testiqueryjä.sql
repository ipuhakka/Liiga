select * from matches where hometeam = 'Kärpät' union select * from matches where awayteam = 'Kärpät';

select * from matches where hometeam = 'Kärpät' union select * from matches where awayteam = 'Kärpät'
union
select * from matches where hometeam = 'TPS' union select * from matches where awayteam = 'TPS';

select * from matches where hometeam = 'Kärpät' union select * from matches where awayteam = 'Kärpät'
and
select * from matches where hometeam = 'TPS' union select * from matches where awayteam = 'TPS'
and
select * from matches where hometeam = 'JYP' union select * from matches where awayteam = 'JYP';

select * from matches where (hometeam='Kärpät' or hometeam='Tappara' or hometeam = 'TPS') 
and 
(awayteam='Kärpät' or awayteam='TPS' or awayteam='Tappara');

INSERT INTO "matches" VALUES('Kärpät', 'Tappara', 2, 1, 1, '2018-03-24', '17-18');

SELECT count(1) FROM matches WHERE hometeam='Kärpät' and awayteam='Tappara' and played_date='2018-03-09';

select * from matches where (played_date >= '2017-03-26'); //

select * from matches where hometeam='HIFK' or awayteam='HIFK' order by played_date limit 1;
