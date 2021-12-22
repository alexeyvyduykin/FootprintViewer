﻿using InteractivitySample.Decorators;
using InteractivitySample.Input.Controller;
using InteractivitySample.Layers;
using Mapsui;
using Mapsui.Geometries.WellKnownText;
using Mapsui.Layers;
using Mapsui.Providers;
using Mapsui.Styles;
using Mapsui.UI;
using ReactiveUI;
using ReactiveUI.Fody.Helpers;
using System;
using System.Linq;

namespace InteractivitySample.ViewModels
{
    public class MainViewModel : ReactiveObject
    {
        private IFeature? _currentFeature;

        private static readonly string wktPolygon = "POLYGON ((991912.3149597258 5524678.959929607, 1075904.3595377824 5661013.872867901, 1170851.8881912378 5615974.6605579285, 1229281.136593364 5707270.36118625, 1375354.2575986795 5691445.773077341, 1398482.5017578548 5613540.108541172, 1352226.013439505 5563631.792197689, 1445956.2660845825 5490595.231695032, 1285275.8329787347 5562414.516189312, 1133116.3319315307 5456511.503460458))";
        private static readonly string wktCircle = "POLYGON ((1768578.3056497877 5141313.400461309, 1774369.9944382226 5141212.306157462, 1780154.6269460102 5140909.14641376, 1785925.1554894939 5140404.290583653, 1791674.5495685253 5139698.353756201, 1797395.8044320454 5138792.196006688, 1803081.9496122948 5137686.92134875, 1808726.0574172547 5136383.876389301, 1814321.251370973 5134884.648687907, 1819860.7145914906 5133191.0648225825, 1825337.6980961624 5131305.1881643925, 1830745.5290242524 5129229.316363546, 1836077.6187667872 5126965.978550071, 1841327.4709937612 5124517.932252451, 1846488.6895689133 5121888.160038007, 1851554.9863424345 5119079.865879099, 1856520.1888121096 5116096.471249572, 1861378.2476435602 5112941.610956228, 1866123.2440404266 5109619.128710364, 1870749.39695551 5106133.072444809, 1875251.0701340872 5102487.689382134, 1879622.7789808202 5098687.420860075, 1883859.197241889 5094736.89692044, 1887955.163494213 5090640.930668116, 1891905.6874338489 5086404.512407047, 1895705.9559559077 5082032.803560314, 1899351.3390185817 5077531.130381737, 1902837.3952841377 5072904.977466653, 1906159.8775300013 5068159.981069787, 1909314.7378233455 5063301.922238337, 1912298.1324528717 5058336.719768661, 1915106.4266117804 5053270.42299514, 1917736.1988262238 5048109.204419988, 1920184.245123844 5042859.352193014, 1922447.5829373195 5037527.262450479, 1924523.4547381655 5032119.431522389, 1926409.331396356 5026642.448017717, 1928102.91526168 5021102.9847972, 1929602.1429630744 5015507.790843481, 1930905.1879225231 5009863.683038522, 1932010.462580462 5004177.537858272, 1932916.6203299747 4998456.282994752, 1933622.5571574261 4992706.88891572, 1934127.4129875337 4986936.360372237, 1934430.5727312355 4981151.72786445, 1934531.6670350817 4975360.039076014, 1934430.5727312355 4969568.350287579, 1934127.4129875337 4963783.717779792, 1933622.5571574261 4958013.189236308, 1932916.6203299747 4952263.795157277, 1932010.462580462 4946542.540293757, 1930905.1879225231 4940856.395113507, 1929602.1429630744 4935212.287308548, 1928102.91526168 4929617.09335483, 1926409.331396356 4924077.6301343115, 1924523.4547381655 4918600.64662964, 1922447.5829373195 4913192.81570155, 1920184.245123844 4907860.725959015, 1917736.1988262238 4902610.873732041, 1915106.4266117807 4897449.655156889, 1912298.1324528717 4892383.358383368, 1909314.7378233455 4887418.155913692, 1906159.8775300013 4882560.097082241, 1902837.3952841377 4877815.100685376, 1899351.339018582 4873188.947770292, 1895705.9559559077 4868687.274591715, 1891905.6874338489 4864315.565744982, 1887955.163494213 4860079.147483913, 1883859.197241889 4855983.181231589, 1879622.77898082 4852032.657291953, 1875251.0701340872 4848232.388769895, 1870749.39695551 4844587.00570722, 1866123.2440404266 4841100.949441665, 1861378.2476435602 4837778.4671958005, 1856520.1888121096 4834623.606902457, 1851554.9863424345 4831640.21227293, 1846488.6895689133 4828831.918114021, 1841327.4709937612 4826202.145899578, 1836077.6187667872 4823754.099601958, 1830745.5290242524 4821490.761788483, 1825337.6980961624 4819414.889987636, 1819860.7145914906 4817529.013329446, 1814321.251370973 4815835.429464122, 1808726.0574172547 4814336.201762727, 1803081.9496122948 4813033.156803279, 1797395.8044320454 4811927.882145341, 1791674.5495685253 4811021.724395827, 1785925.155489494 4810315.7875683755, 1780154.6269460104 4809810.931738269, 1774369.9944382226 4809507.771994567, 1768578.3056497877 4809406.67769072, 1762786.6168613527 4809507.771994567, 1757001.9843535651 4809810.931738269, 1751231.4558100812 4810315.7875683755, 1745482.0617310498 4811021.724395827, 1739760.8068675299 4811927.882145341, 1734074.6616872805 4813033.156803279, 1728430.5538823206 4814336.201762727, 1722835.3599286024 4815835.429464122, 1717295.8967080845 4817529.013329446, 1711818.913203413 4819414.889987636, 1706411.0822753229 4821490.761788483, 1701078.992532788 4823754.099601958, 1695829.1403058143 4826202.145899578, 1690667.921730662 4828831.918114021, 1685601.6249571408 4831640.21227293, 1680636.4224874657 4834623.606902457, 1675778.3636560151 4837778.4671958005, 1671033.3672591487 4841100.949441665, 1666407.2143440656 4844587.00570722, 1661905.541165488 4848232.388769895, 1657533.832318755 4852032.657291953, 1653297.4140576862 4855983.181231589, 1649201.4478053623 4860079.147483913, 1645250.9238657265 4864315.565744982, 1641450.6553436676 4868687.274591715, 1637805.2722809934 4873188.947770292, 1634319.2160154376 4877815.100685376, 1630996.733769574 4882560.097082241, 1627841.8734762298 4887418.155913692, 1624858.4788467037 4892383.358383368, 1622050.184687795 4897449.655156889, 1619420.4124733515 4902610.873732041, 1616972.3661757314 4907860.725959015, 1614709.0283622558 4913192.81570155, 1612633.1565614098 4918600.64662964, 1610747.2799032193 4924077.6301343115, 1609053.6960378953 4929617.09335483, 1607554.468336501 4935212.287308548, 1606251.4233770522 4940856.395113507, 1605146.1487191133 4946542.540293757, 1604239.9909696009 4952263.795157277, 1603534.0541421492 4958013.1892363075, 1603029.1983120416 4963783.717779792, 1602726.0385683398 4969568.350287579, 1602624.9442644936 4975360.039076014, 1602726.0385683398 4981151.727864449, 1603029.1983120416 4986936.360372237, 1603534.0541421492 4992706.88891572, 1604239.9909696006 4998456.282994752, 1605146.1487191133 5004177.537858272, 1606251.4233770522 5009863.683038522, 1607554.468336501 5015507.790843481, 1609053.6960378953 5021102.984797199, 1610747.2799032193 5026642.448017717, 1612633.1565614098 5032119.431522389, 1614709.0283622558 5037527.262450479, 1616972.3661757314 5042859.352193014, 1619420.4124733515 5048109.204419988, 1622050.1846877947 5053270.42299514, 1624858.4788467037 5058336.719768661, 1627841.8734762298 5063301.922238337, 1630996.733769574 5068159.981069787, 1634319.2160154376 5072904.977466653, 1637805.2722809934 5077531.130381737, 1641450.6553436676 5082032.803560314, 1645250.9238657265 5086404.512407047, 1649201.4478053623 5090640.930668116, 1653297.4140576862 5094736.89692044, 1657533.832318755 5098687.420860075, 1661905.541165488 5102487.689382134, 1666407.2143440654 5106133.072444809, 1671033.3672591487 5109619.128710364, 1675778.363656015 5112941.610956228, 1680636.4224874654 5116096.471249572, 1685601.6249571405 5119079.865879098, 1690667.921730662 5121888.160038007, 1695829.140305814 5124517.932252451, 1701078.992532788 5126965.978550071, 1706411.0822753229 5129229.316363546, 1711818.913203413 5131305.1881643925, 1717295.8967080847 5133191.0648225825, 1722835.3599286024 5134884.648687907, 1728430.5538823206 5136383.876389301, 1734074.6616872805 5137686.92134875, 1739760.8068675299 5138792.196006688, 1745482.0617310498 5139698.353756201, 1751231.4558100812 5140404.290583653, 1757001.984353565 5140909.14641376, 1762786.6168613527 5141212.306157462))";
        private static readonly string wktRectangle = "POLYGON ((1199513.0851808232 5265186.30704513, 1707054.9529943874 5265186.30704513, 1707054.9529943874 5158785.963672167, 1199513.0851808232 5158785.963672167))";
        private static readonly string wktRoute = "LINESTRING (1042158.9502034901 5321132.10610815, 1154674.2558392682 5217177.747640314, 966333.4181445962 5190271.913683931, 1025037.0558676108 5066749.675975089, 922305.6898523355 4964018.309959814, 1080071.7162329375 4973802.249580316, 951657.508713843 4740210.691140819, 1236614.7501609768 5054519.751449459, 1072733.761517561 5145021.192939107)";

        private static readonly Color PolygonBackgroundColor = new Color(20, 120, 120, 40);
        private static readonly Color PolygonLineColor = new Color(20, 120, 120, 255);
        private static readonly Color PolygonOutlineColor = new Color(20, 20, 20, 255);

        private static readonly Color CircleBackgroundColor = new Color(20, 120, 120, 40);
        private static readonly Color CircleLineColor = new Color(20, 120, 120, 255);
        private static readonly Color CircleOutlineColor = new Color(20, 20, 20, 255);

        private static readonly Color RectangleBackgroundColor = new Color(20, 120, 120, 40);
        private static readonly Color RectangleLineColor = new Color(20, 120, 120, 255);
        private static readonly Color RectangleOutlineColor = new Color(20, 20, 20, 255);

        private static readonly Color RouteBackgroundColor = new Color(20, 120, 120, 40);
        private static readonly Color RouteLineColor = new Color(20, 120, 120, 255);
        private static readonly Color RouteOutlineColor = new Color(20, 20, 20, 255);

        public MainViewModel()
        {
            Map = CreateMap();
            MapListener = new MapListener();

            MapListener.LeftClickOnMap += MapListener_LeftClickOnMap;

            this.WhenAnyValue(s => s.IsSelect).Subscribe((_) =>
            {
                _currentFeature = null;

                InteractiveLayerRemove();

                if (IsSelect == true)
                {
                    IsTranslate = false;
                    IsRotate = false;
                    IsScale = false;
                    IsEdit = false;
                }
            });

            this.WhenAnyValue(s => s.IsTranslate).Subscribe((_) =>
            {
                _currentFeature = null;

                InteractiveLayerRemove();

                if (IsTranslate == true)
                {
                    IsSelect = false;
                    IsRotate = false;
                    IsScale = false;
                    IsEdit = false;
                }
            });

            this.WhenAnyValue(s => s.IsRotate).Subscribe((_) =>
            {
                _currentFeature = null;

                InteractiveLayerRemove();

                if (IsRotate == true)
                {
                    IsTranslate = false;
                    IsSelect = false;
                    IsScale = false;
                    IsEdit = false;
                }
            });

            this.WhenAnyValue(s => s.IsScale).Subscribe((_) =>
            {
                _currentFeature = null;

                InteractiveLayerRemove();

                if (IsScale == true)
                {
                    IsTranslate = false;
                    IsRotate = false;
                    IsSelect = false;
                    IsEdit = false;
                }
            });

            this.WhenAnyValue(s => s.IsEdit).Subscribe((_) =>
            {
                _currentFeature = null;

                InteractiveLayerRemove();

                if (IsEdit == true)
                {
                    IsTranslate = false;
                    IsRotate = false;
                    IsScale = false;
                    IsSelect = false;
                }
            });

            ActualController = new EditController();
        }

        private void InteractiveLayerRemove()
        {
            var layer = Map.Layers.FindLayer("InteractiveLayer").FirstOrDefault();

            if (layer != null)
            {
                Map.Layers.Remove(layer);
            }
        }

        private void MapListener_LeftClickOnMap(object? sender, EventArgs e)
        {
            if (sender is MapInfo mapInfo)
            {
                var feature = mapInfo.Feature;

                if (IsSelect == true)
                {
                    InteractiveLayerRemove();
        
                    if (feature != _currentFeature)
                    {
                        Map.Layers.Add(CreateSelectLayer(mapInfo.Layer, mapInfo.Feature));

                        _currentFeature = feature;
                    }
                    else
                    {
                        _currentFeature = null;
                    }

                    return;
                }
                
                IDecorator? decorator = null;

                if (IsScale == true)
                {
                    decorator = new ScaleDecorator(feature);
                }
                if (IsTranslate == true)
                {
                    decorator = new TranslateDecorator(feature);
                }
                if (IsRotate == true)
                {
                    decorator = new RotateDecorator(feature);
                }
                if (IsEdit == true)
                {
                    decorator = new EditDecorator(feature);
                }

                if (decorator == null)
                {
                    return;
                }

                InteractiveLayerRemove();

                if (feature != _currentFeature)
                {
                    Map.Layers.Add(CreateDecoratorLayer(mapInfo.Layer, decorator));

                    MapObserver = new MapObserver();

                    MapObserver.Started += (s, e) =>
                    {
                        var vertices = decorator.GetActiveVertices();

                        var vertexTouched = vertices.OrderBy(v => v.Distance(e.WorldPosition)).FirstOrDefault(v => v.Distance(e.WorldPosition) < e.ScreenDistance);

                        if (vertexTouched != null)
                        {
                            decorator.Starting(e.WorldPosition);
                        }
                    };

                    MapObserver.Delta += (s, e) =>
                    {
                        decorator.Moving(e.WorldPosition);
                    };

                    MapObserver.Completed += (s, e) =>
                    {
                        decorator.Ending();
                    };

                    _currentFeature = feature;
                }
                else
                {
                    _currentFeature = null;
                }
            }
        }

        public static Map CreateMap()
        {
            var map = new Map()
            {
                CRS = "EPSG:3857",
                // Transformation = new MinimalTransformation(),
            };

            CustomProvider provider = new CustomProvider();
            provider.AddFeature("Polygon", wktPolygon/*, CreatePolygonStyle()*/);
            provider.AddFeature("Circle", wktCircle/*, CreateCircleStyle()*/);
            provider.AddFeature("Rectangle", wktRectangle/*, CreateRectangleStyle()*/);
            provider.AddFeature("Route", wktRoute/*, CreateRouteStyle()*/);

            var userLayer = CreateLayer(provider);

            map.Layers.Add(new Layer()); // BackgroundLayer
            map.Layers.Add(userLayer);

            return map;
        }

        private static ILayer CreateSelectLayer(ILayer source, IFeature feature)
        {
            return new SelectLayer(source, feature)
            {
                Name = "InteractiveLayer",
                IsMapInfoLayer = true,
                Style = new VectorStyle()
                {
                    Fill = new Brush(Color.Transparent),
                    Outline = new Pen(Color.Green, 4),
                    Line = new Pen(Color.Green, 4),
                },
            };
        }

        private static ILayer CreateDecoratorLayer(ILayer source, IDecorator decorator)
        {
            return new InteractiveLayer(source, decorator)
            {
                Name = "InteractiveLayer",
                IsMapInfoLayer = true,
                Style = new SymbolStyle()
                {
                    Fill = new Brush(Color.White),
                    Outline = new Pen(Color.Black, 2 / 0.3),
                    Line = null,//new Pen(Color.Black, 2),
                    SymbolType = SymbolType.Ellipse,
                    SymbolScale = 0.3,
                },
            };
        }

        private static ILayer CreateLayer(IProvider provider)
        {
            var polygonLayer = new Layer
            {                            
                DataSource = provider,
                IsMapInfoLayer = true,
                Style = CreateStyle(),
            };

            return polygonLayer;
        }

        private static IStyle CreateStyle()
        {
            return new VectorStyle
            {
                Fill = new Brush(new Color(PolygonBackgroundColor)),
                Line = new Pen(PolygonLineColor, 3),
                Outline = new Pen(PolygonOutlineColor, 3)
            };
        }

        [Reactive]
        public Map Map { get; set; }

        [Reactive]
        public MapListener? MapListener { get; set; }

        [Reactive]
        public bool IsSelect { get; set; } = false;

        [Reactive]
        public bool IsTranslate { get; set; } = false;

        [Reactive]
        public bool IsRotate { get; set; } = false;

        [Reactive]
        public bool IsScale { get; set; } = false;

        [Reactive]
        public bool IsEdit { get; set; } = false;

        [Reactive]
        public IController ActualController { get; set; }

        [Reactive]
        public IMapObserver? MapObserver { get; set; }
    }
}
