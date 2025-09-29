import { Component } from '@angular/core';
import { NzCardModule } from 'ng-zorro-antd/card';
import { NzStatisticModule } from 'ng-zorro-antd/statistic';
import { NzGridModule } from 'ng-zorro-antd/grid';
import { NzIconModule } from 'ng-zorro-antd/icon';

@Component({
  selector: 'app-dashboard',
  standalone: true,
  imports: [
    NzCardModule,
    NzStatisticModule,
    NzGridModule,
    NzIconModule
  ],
  templateUrl: './dashboard.html',
  styleUrl: './dashboard.css'
})
export class DashboardComponent {
  stats = [
    {
      title: 'Tổng sản phẩm',
      value: 125,
      icon: 'shopping',
      color: '#1890ff'
    },
    {
      title: 'Đơn hàng hôm nay',
      value: 42,
      icon: 'file-text',
      color: '#52c41a'
    },
    {
      title: 'Doanh thu tháng',
      value: 15680000,
      icon: 'dollar',
      color: '#faad14'
    },
    {
      title: 'Khách hàng',
      value: 89,
      icon: 'user',
      color: '#f5222d'
    }
  ];
}
